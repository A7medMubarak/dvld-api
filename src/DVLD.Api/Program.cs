using DVLD.Api.Extensions;
using DVLD.Api.Middleware;
using Serilog;

namespace DVLD.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ── Serilog (catches startup errors + full app) ──
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Information)
                .WriteTo.Console()
                .WriteTo.Logger(lc => lc
                    .Filter.ByExcluding(IsSqlCommandLogEvent)
                    .WriteTo.Async(a => a.File("logs/dvld-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 14)))
                .WriteTo.Async(a => a.File("logs/security-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning))
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(IsSqlCommandLogEvent)
                    .WriteTo.Async(a => a.File("logs/sql-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 30)))
                .CreateLogger();

            try
            {
                // Create WebApplicationBuilder.
                var builder = WebApplication.CreateBuilder(args);

                // Use pre-configured Log.Logger as the app-wide logger
                builder.Host.UseSerilog();

                // ========== Add Services ==========
                builder.Services.AddControllers();

                // Swagger
                builder.Services.AddSwagger();

                // CORS
                builder.Services.AddCorsPolicy();

                // Database
                builder.Services.AddDatabase(builder.Configuration);

                // Global Exception Handling
                builder.Services.AddGlobalExceptionHandling();

                // Validation
                builder.Services.AddValidation();

                // Rate Limiting
                builder.Services.AddRateLimiting();

                // Repositories & Services
                builder.Services.AddRepositories();
                builder.Services.AddServices();

                // JWT Authentication
                builder.Services.AddJwtAuthentication(builder.Configuration);

                // ========== Build App ==========
                var app = builder.Build();

                // ========== Middleware Pipeline ==========
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseMiddleware<RequestLoggingMiddleware>();
                app.UseExceptionHandler();
                app.UseHttpsRedirection();
                app.UseCors("DVLDApiCorsPolicy");
                app.UseAuthentication();
                app.UseRateLimiter();
                app.UseAuthorization();

                app.MapControllers();
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static bool IsSqlCommandLogEvent(Serilog.Events.LogEvent e)
        {
            return e.Properties.TryGetValue("SourceContext", out var sc)
                && sc.ToString().Contains("Microsoft.EntityFrameworkCore.Database.Command");
        }
    }
}
