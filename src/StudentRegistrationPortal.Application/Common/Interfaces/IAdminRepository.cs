using StudentRegistrationPortal.Application.DTOs;
using StudentRegistrationPortal.Domain.Entities;

namespace StudentRegistrationPortal.Application.Common.Interfaces;

public interface IAdminRepository
{
    Task<IReadOnlyList<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminEnrollmentDetailsDto>> GetAllEnrollmentsAsync(int? statusId = null, CancellationToken cancellationToken = default);
    Task<bool> UpdateEnrollmentStatusAsync(int enrollmentId, int statusId, CancellationToken cancellationToken = default);
}
