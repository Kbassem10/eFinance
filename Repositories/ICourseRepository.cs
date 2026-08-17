using StudentRegistrationPortal.Api.DTOs;

namespace StudentRegistrationPortal.Api.Repositories;

public interface ICourseRepository
{
    Task<IReadOnlyList<CourseDetailsDto>> GetAllCoursesAsync(CancellationToken cancellationToken = default);
    Task<CourseDetailsDto?> GetCourseByIdAsync(int courseId, CancellationToken cancellationToken = default);
    Task<CourseDetailsDto?> GetCourseByCodeAsync(string courseCode, CancellationToken cancellationToken = default);
}
