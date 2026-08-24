using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudentRegistrationPortal.Application.Common.Interfaces;
using StudentRegistrationPortal.Domain.Entities;

namespace StudentRegistrationPortal.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public string GenerateToken(User user, string role, int? studentId = null)
    {
        var secret = _config["Jwt:Secret"] 
            ?? "SuperSecretKeyForStudentRegistrationPortal2026SecureAuthentication!";
        var issuer = _config["Jwt:Issuer"] ?? "StudentRegistrationPortal";
        var audience = _config["Jwt:Audience"] ?? "StudentRegistrationPortalClients";
        var expirationMinutes = Convert.ToDouble(_config["Jwt:ExpirationInMinutes"] ?? "120");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, role)
        };

        if (studentId.HasValue)
        {
            claims.Add(new Claim("StudentId", studentId.Value.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

