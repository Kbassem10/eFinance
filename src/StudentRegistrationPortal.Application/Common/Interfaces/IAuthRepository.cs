using StudentRegistrationPortal.Application.DTOs;
using StudentRegistrationPortal.Domain.Entities;

namespace StudentRegistrationPortal.Application.Common.Interfaces;

public interface IAuthRepository
{
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetUserRolesAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<int>> GetUserRoleIdsAsync(int userId, CancellationToken cancellationToken = default);
    Task<StudentDetailsDto?> GetStudentByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<UserDetailsDto?> GetUserDetailsByIdAsync(int userId, CancellationToken cancellationToken = default);
}
