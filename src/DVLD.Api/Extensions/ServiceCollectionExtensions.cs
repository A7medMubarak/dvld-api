using DVLD.Contracts.Options;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using DVLD.Api.Services;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.OpenApi.Models;
using DVLD.Api.Middleware;
using FluentValidation;
using DVLD.Contracts.Requests.LicenseClass;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace DVLD.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                       .EnableSensitiveDataLogging());

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<ILicenseRepository, LicenseRepository>();
            services.AddScoped<IApplicationTypeRepository, ApplicationTypeRepository>();
            services.AddScoped<ILicenseClassRepository, LicenseClassRepository>();
            services.AddScoped<ITestRepository, TestRepository>();
            services.AddScoped<ITestTypeRepository, TestTypeRepository>();
            services.AddScoped<ILocalLicenseAppRepository, LocalLicenseAppRepository>();
            services.AddScoped<ITestAppointmentRepository, TestAppointmentRepository>();
            services.AddScoped<IDetainedLicenseRepository, DetainedLicenseRepository>();
            services.AddScoped<IInternationalLicenseRepository, InternationalLicenseRepository>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<ILicenseService, LicenseService>();
            services.AddScoped<IApplicationTypeService, ApplicationTypeService>();
            services.AddScoped<ILicenseClassService, LicenseClassService>();
            services.AddScoped<ITestService, TestService>();
            services.AddScoped<ITestTypeService, TestTypeService>();
            services.AddScoped<ILocalLicenseAppService, LocalLicenseAppService>();
            services.AddScoped<ITestAppointmentService, TestAppointmentService>();
            services.AddScoped<IDetainedLicenseService, DetainedLicenseService>();
            services.AddScoped<IInternationalLicenseService, InternationalLicenseService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ISecurityAuditService, SecurityAuditService>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("JwtSettings");            
           
            services.Configure<JwtSettings>(jwtSection);

            var jwtSettings = jwtSection.Get<JwtSettings>()
                ?? throw new InvalidOperationException("JWT settings are missing.");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("DVLDApiCorsPolicy", policy =>
                {
                    policy.WithOrigins("https://localhost:7247", "http://localhost:5003")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddGlobalExceptionHandling(this IServiceCollection services)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
            
            return services;
        }

        public static IServiceCollection AddValidation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<LicenseClassWriteRequest>();

            return services;
        }

        public static IServiceCollection AddRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // ?? Global ??
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
                {
                    var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (userId == null)
                    {
                        var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                        return RateLimitPartition.GetFixedWindowLimiter($"anon_{ip}", _ => new()
                        {
                            PermitLimit = 30,
                            Window = TimeSpan.FromMinutes(1)
                        });
                    }
                    return RateLimitPartition.GetFixedWindowLimiter($"user_{userId}", _ => new()
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1)
                    });
                });

                // ?? Auth (Login/Refresh) ??
                options.AddPolicy("Auth", context =>
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new()
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1)
                    });
                });

                // ?? Sensitive (ChangePassword) ??
                options.AddPolicy("Sensitive", context =>
                {
                    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (userId == null)
                    {
                        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                        return RateLimitPartition.GetFixedWindowLimiter($"sensitive_anon_{ip}", _ => new()
                        {
                            PermitLimit = 3,
                            Window = TimeSpan.FromMinutes(1)
                        });
                    }
                    return RateLimitPartition.GetFixedWindowLimiter($"sensitive_{userId}", _ => new()
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1)
                    });
                });

                // ?? 429 response ??
                options.OnRejected = async (ctx, ct) =>
                {
                    ctx.HttpContext.Response.StatusCode = 429;
                    ctx.HttpContext.Response.ContentType = "application/problem+json";
                    await ctx.HttpContext.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Status = 429,
                        Title = "Too Many Try again later"
                    }, ct);
                };
            });
            return services;
        }
    }
}
