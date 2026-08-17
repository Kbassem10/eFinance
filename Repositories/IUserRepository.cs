using StudentRegistrationPortal.Api.DTOs;
using StudentRegistrationPortal.Api.Entities;

namespace StudentRegistrationPortal.Api.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyList<UserDetailsDto>> GetAllUserDetailsAsync(CancellationToken cancellationToken = default);
    Task<UserDetailsDto?> GetUserDetailsByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<UserDetailsDto?> GetUserDetailsByEmailAsync(string email, CancellationToken cancellationToken = default);
}
