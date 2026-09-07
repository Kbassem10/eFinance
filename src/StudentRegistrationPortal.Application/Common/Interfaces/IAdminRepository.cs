using StudentRegistrationPortal.Application.DTOs;
using StudentRegistrationPortal.Domain.Entities;

namespace StudentRegistrationPortal.Application.Common.Interfaces;

public interface IAdminRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<AdminDetailsDto?> GetUserDetailsByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetUserRolesAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<int>> GetUserRoleIdsAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminEnrollmentDetailsDto>> GetAllEnrollmentsAsync(int? statusId = null, CancellationToken cancellationToken = default);
    Task<bool> UpdateEnrollmentStatusAsync(int enrollmentId, int statusId, CancellationToken cancellationToken = default);
}
