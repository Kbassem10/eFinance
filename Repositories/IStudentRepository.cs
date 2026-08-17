using StudentRegistrationPortal.Api.DTOs;

namespace StudentRegistrationPortal.Api.Repositories;

public interface IStudentRepository
{
    Task<IReadOnlyList<StudentDetailsDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StudentDetailsDto?> GetByIdAsync(int studentId, CancellationToken cancellationToken = default);
    Task<StudentDetailsDto?> GetByStudentNumberAsync(string studentNumber, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateStudentDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int studentId, UpdateStudentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int studentId, CancellationToken cancellationToken = default);
    Task<int> GetTotalCreditHoursAsync(int studentId, int semesterId, CancellationToken cancellationToken = default);
}
