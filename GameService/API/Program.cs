using API.Middleware;
using Application;
using Application.Interfaces.Cache;
using Application.Services.WorldService;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Realtime;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ─────────────────────────────
            // SERVICES
            // ─────────────────────────────
            builder.Services.AddLogging();
            builder.Services.AddControllers();
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure();

            // ─────────────────────────────
            // CORS
            // ─────────────────────────────
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("StudioCorsPolicy", policy =>
                {
                    policy.SetIsOriginAllowed(origin => true)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // ─────────────────────────────
            // SWAGGER
            // ─────────────────────────────
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                        return new[] { api.GroupName };

                    var controller = api.ActionDescriptor.RouteValues["controller"];
                    return new[] { controller! };
                });
            });

            // ─────────────────────────────
            // JWT AUTH
            // ─────────────────────────────
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")!;
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!;
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")!;

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = issuer,
                        ValidAudience = audience,

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey)
                        ),

                        ClockSkew = TimeSpan.FromSeconds(15)
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            Console.WriteLine($"[JWT] Request Path: {path}");

                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/hubs/game") ||
                                 path.StartsWithSegments("/hubs/admin")))
                            {
                                context.Token = accessToken;
                                Console.WriteLine("[JWT] Access token received from SignalR query string.");
                            }
                            else
                            {
                                Console.WriteLine("[JWT] No SignalR access token found.");
                            }

                            return Task.CompletedTask;
                        },

                        OnTokenValidated = context =>
                        {
                            Console.WriteLine("[JWT] Token validated successfully.");

                            foreach (var claim in context.Principal!.Claims)
                            {
                                Console.WriteLine($"[JWT] Claim: {claim.Type} = {claim.Value}");
                            }

                            return Task.CompletedTask;
                        },

                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine("[JWT] Authentication FAILED!");
                            Console.WriteLine(context.Exception);

                            return Task.CompletedTask;
                        },

                        OnChallenge = context =>
                        {
                            Console.WriteLine("[JWT] Authorization Challenge");

                            Console.WriteLine($"Error: {context.Error}");
                            Console.WriteLine($"Description: {context.ErrorDescription}");

                            return Task.CompletedTask;
                        },

                        OnForbidden = context =>
                        {
                            Console.WriteLine("[JWT] Forbidden (authenticated but authorization failed).");

                            if (context.HttpContext.User.Identity?.IsAuthenticated == true)
                            {
                                foreach (var claim in context.HttpContext.User.Claims)
                                {
                                    Console.WriteLine($"[JWT] User Claim: {claim.Type} = {claim.Value}");
                                }
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddAuthorization();

            // ─────────────────────────────
            // BUILD
            // ─────────────────────────────
            var app = builder.Build();

            // ─────────────────────────────
            // MIGRATE
            // ─────────────────────────────
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<RelationalDB>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                // 1. Check for the wipe flag BEFORE attempting any migrations
                if (Environment.GetEnvironmentVariable("WIPE_DB") == "true")
                {
                    logger.LogWarning("WIPE_DB flag detected! Dropping the database entirely...");

                    // This completely wipes the database schema and data
                    db.Database.EnsureDeleted();

                    logger.LogInformation("Database dropped successfully. Proceeding to rebuild via migrations...");
                }

                var retries = 5;
                while (retries > 0)
                {
                    try
                    {
                        logger.LogInformation("Applying database migrations...");

                        // 2. This will now recreate the DB from scratch (if we just dropped it) 
                        // and apply all your migration files cleanly.
                        db.Database.Migrate();

                        logger.LogInformation("Database migration completed.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        retries--;
                        logger.LogWarning(ex, "Migration failed. Retrying...");

                        if (retries == 0)
                            throw;

                        Thread.Sleep(5000);
                    }
                }
            }

            // ─────────────────────────────
            // SEEDING DATA
            // ─────────────────────────────
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<RelationalDB>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                try
                {
                    await DataSeeder.SeedAsync(db);

                    logger.LogInformation("Database seeding completed successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Database seeding failed.");
                }
            }

            // ─────────────────────────────
            // LOAD CACHE
            // ─────────────────────────────
            using (var scope = app.Services.CreateScope())
            {
                var loader = scope.ServiceProvider.GetRequiredService<ICacheProvider>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("Caching definition/meta data...");
                await loader.LoadAllAsync();
                logger.LogInformation("Metadata caching completed.");
            }

            // ─────────────────────────────
            // WORLD BOOTSTRAP
            // ─────────────────────────────
            using (var scope = app.Services.CreateScope())
            {
                var bootstrap = scope.ServiceProvider.GetRequiredService<BootstrapService>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("Reloading world data...");
                await bootstrap.LoadAsync();
                logger.LogInformation("World reloading completed.");
            }

            // ─────────────────────────────
            // SIGNALR HUB
            // ─────────────────────────────
            app.MapHub<GameHub>("/hubs/game");
            app.MapHub<AdminHub>("/hubs/admin");

            // ─────────────────────────────
            // MIDDLEWARE PIPELINE
            // ─────────────────────────────
            app.UseMiddleware<GlobalExceptionHandler>();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors("StudioCorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}