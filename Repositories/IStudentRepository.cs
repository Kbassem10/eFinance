using StudentRegistrationPortal.Api.DTOs;

namespace StudentRegistrationPortal.Api.Repositories;

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
}
