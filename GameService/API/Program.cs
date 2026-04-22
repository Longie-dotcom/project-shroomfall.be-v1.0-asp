using API.Middleware;
using Application;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ─────────────────────────────
            // SERVICES
            // ─────────────────────────────
            builder.Services.AddControllers();
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure();

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
            // MIDDLEWARE PIPELINE
            // ─────────────────────────────
            app.UseMiddleware<GlobalExceptionHandler>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}