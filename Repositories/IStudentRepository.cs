using StudentRegistrationPortal.Api.Data;
using StudentRegistrationPortal.Api.DTOs;
using StudentRegistrationPortal.Api.Entities;

namespace StudentRegistrationPortal.Api.Repositories;

public interface IStudentRepository
{
    Task<IReadOnlyList<StudentDetailsDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StudentDetailsDto?> GetByIdAsync(int studentId, CancellationToken cancellationToken = default);
    Task<StudentDetailsDto?> GetByStudentNumberAsync(string studentNumber, CancellationToken cancellationToken = default);
    Task<StoredProcedureResult> CreateViaStoredProcedureAsync(CreateStudentDto dto, CancellationToken cancellationToken = default);
    Task<StoredProcedureResult> UpdateViaStoredProcedureAsync(int studentId, UpdateStudentDto dto, CancellationToken cancellationToken = default);
    Task<StoredProcedureResult> DeleteViaStoredProcedureAsync(int studentId, CancellationToken cancellationToken = default);
    Task<int> GetTotalCreditHoursAsync(int studentId, int semesterId, CancellationToken cancellationToken = default);
}
