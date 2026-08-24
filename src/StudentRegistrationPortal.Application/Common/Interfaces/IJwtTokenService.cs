using StudentRegistrationPortal.Domain.Entities;

namespace StudentRegistrationPortal.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user, string role, int? studentId = null);
}

