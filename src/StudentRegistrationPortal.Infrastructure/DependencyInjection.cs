using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using StudentRegistrationPortal.Application.Common.Interfaces;
using StudentRegistrationPortal.Infrastructure.Persistence.DbContext;
using StudentRegistrationPortal.Infrastructure.Persistence.Migrations;
using StudentRegistrationPortal.Infrastructure.Persistence.Repositories;
using StudentRegistrationPortal.Infrastructure.Services;

namespace StudentRegistrationPortal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var rawConnectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        var builder = new MySqlConnectionStringBuilder(rawConnectionString)
        {
            AllowUserVariables = true,
            UseAffectedRows = false
        };
        var connectionString = builder.ConnectionString;

        // 1. MySQL ADO.NET Data Source
        services.AddMySqlDataSource(connectionString);

        // 2. EF Core DbContext Setup (Ready for EF Core & LINQ)
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var serverVersion = ServerVersion.AutoDetect(connectionString);
            options.UseMySql(connectionString, serverVersion);
        });

        // 3. ADO.NET Repositories & Unit Of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IStudentRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Students);
        services.AddScoped<IUserRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Users);
        services.AddScoped<ICoursesRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Courses);
        services.AddScoped<IAdminRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Admin);
        services.AddScoped<ILookupRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Lookups);
        services.AddScoped<IAuthRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Auth);

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IDatabaseMigrator, DatabaseMigrator>();

        // JWT Authentication Configuration
        var jwtSecret = configuration["Jwt:Secret"] 
            ?? "SuperSecretKeyForStudentRegistrationPortal2026SecureAuthentication!";
        var jwtIssuer = configuration["Jwt:Issuer"] ?? "StudentRegistrationPortal";
        var jwtAudience = configuration["Jwt:Audience"] ?? "StudentRegistrationPortalClients";

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
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
            };
        });

        services.AddAuthorization();

        return services;
    }
}
