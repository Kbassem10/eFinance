using StudentRegistrationPortal.Application.DTOs;

namespace StudentRegistrationPortal.Application.Common.Interfaces;

public interface ILookupRepository
{
    Task<IReadOnlyList<LookupItemDto>> GetDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LookupItemDto>> GetSemestersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LookupItemDto>> GetInstructorsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LookupItemDto>> GetRoomsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LookupItemDto>> GetCourseStatusesAsync(CancellationToken cancellationToken = default);
}
