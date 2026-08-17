using StudentRegistrationPortal.Api.DTOs;

namespace StudentRegistrationPortal.Api.Repositories;

public interface IStudentRepository
{
    Task<IReadOnlyList<StudentDetailsDto>> GetAllAsync();
    Task<StudentDetailsDto?> GetByIdAsync(int studentId);
    Task<StudentDetailsDto?> GetByStudentNumberAsync(string studentNumber);
    Task<int> CreateAsync(CreateStudentDto dto);
    Task<bool> UpdateAsync(int studentId, UpdateStudentDto dto);
    Task<bool> DeleteAsync(int studentId);
    Task<int> GetTotalCreditHoursAsync(int studentId, int semesterId);
}
