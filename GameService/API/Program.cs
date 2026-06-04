using API.Middleware;
using Application;
using Application.Bootstrapper;
using Application.Interfaces.Cache;
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

                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/hub/game"))
                            {
                                context.Token = accessToken;
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

                var retries = 5;
                while (retries > 0)
                {
                    try
                    {
                        logger.LogInformation("Applying database migrations...");
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
            // INITIALIZE DATA
            // ─────────────────────────────
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<RelationalDB>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("Seeding initial data...");
                await DataInitializer.SeedAsync(db);
                logger.LogInformation("Data seeding completed.");
            }

            // ─────────────────────────────
            // LOAD CACHE
            // ─────────────────────────────
            using (var scope = app.Services.CreateScope())
            {
                var loader = scope.ServiceProvider.GetRequiredService<ICacheLoader>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("Caching definition/meta data...");
                await loader.LoadAllAsync();
                logger.LogInformation("Metadata caching completed.");
            }

            // ─────────────────────────────
            // TOPOLOGY BOOTSTRAP
            // ─────────────────────────────
            using (var scope = app.Services.CreateScope())
            {
                var bootstrap = scope.ServiceProvider.GetRequiredService<TopologyBootstrap>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("Reloading topology data...");
                await bootstrap.LoadAsync();
                logger.LogInformation("Topology reloading completed.");
            }

            // ─────────────────────────────
            // SIGNALR HUB
            // ─────────────────────────────
            app.MapHub<GameHub>("/hubs/game");

            // ─────────────────────────────
            // MIDDLEWARE PIPELINE
            // ─────────────────────────────
            app.UseMiddleware<GlobalExceptionHandler>();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}