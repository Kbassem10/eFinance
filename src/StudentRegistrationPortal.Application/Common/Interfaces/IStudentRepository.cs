using StudentRegistrationPortal.Application.DTOs;

namespace StudentRegistrationPortal.Application.Common.Interfaces;

public interface IStudentRepository
{
    Task<IReadOnlyList<StudentDetailsDto>> GetAllAsync();
    Task<StudentDetailsDto?> GetByIdAsync(int studentId);
    Task<StudentDetailsDto?> GetByStudentNumberAsync(string studentNumber);
    Task<StudentDetailsDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<StudentDetailsDto?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(int userId, CreateStudentDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int studentId, UpdateStudentDto dto);
    Task<bool> DeleteAsync(int studentId);
    Task<int> GetTotalCreditHoursAsync(int studentId, int semesterId);
    Task<IReadOnlyList<EnrollmentResultDto>> EnrollInCoursesAsync(int studentId, IReadOnlyList<int> courseIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EnrollmentResultDto>> GetStudentEnrollmentsAsync(int studentId, EnrollmentFilterDto? filter = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminEnrollmentDetailsDto>> GetAllEnrollmentsAsync(int? statusId = null, CancellationToken cancellationToken = default);
    Task<bool> UpdateEnrollmentStatusAsync(int enrollmentId, int statusId, CancellationToken cancellationToken = default);
}

