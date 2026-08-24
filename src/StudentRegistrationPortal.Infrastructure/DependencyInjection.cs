using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using StudentRegistrationPortal.Application.Common.Interfaces;
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
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        // MySQL ADO.NET Data Source
        services.AddMySqlDataSource(connectionString);

        // Repositories & Unit Of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
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

