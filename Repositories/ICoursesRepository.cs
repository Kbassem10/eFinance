using StudentRegistrationPortal.Api.DTOs;

namespace StudentRegistrationPortal.Api.Repositories;

public interface ICoursesRepository
{
    Task<IReadOnlyList<CourseDetailsDto>> GetAllAsync(int? departmentId = null, int? statusId = null, CancellationToken cancellationToken = default);
    Task<CourseDetailsDto?> GetByIdAsync(int courseId, CancellationToken cancellationToken = default);
    Task<CourseDetailsDto?> GetByCourseCodeAsync(string courseCode, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateCourseDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int courseId, UpdateCourseDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CourseDetailsDto>> GetPrerequisitesAsync(int courseId, CancellationToken cancellationToken = default);
}
