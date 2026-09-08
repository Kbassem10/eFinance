using StudentRegistrationPortal.Application.DTOs;

namespace StudentRegistrationPortal.Application.Common.Interfaces;

public interface ILookupRepository
{
    Task<IReadOnlyList<LookupItemDto>> GetDepartmentsAsync(string departmentCode = "", CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LookupItemDto>> GetSemestersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LookupItemDto>> GetInstructorsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LookupItemDto>> GetRoomsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LookupItemDto>> GetCourseStatusesAsync(CancellationToken cancellationToken = default);

    // Full Management Methods
    Task<IReadOnlyList<DepartmentDetailsDto>> GetAllDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<DepartmentDetailsDto?> GetDepartmentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateDepartmentAsync(CreateUpdateDepartmentDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateDepartmentAsync(int id, CreateUpdateDepartmentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteDepartmentAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SemesterDetailsDto>> GetAllSemestersAsync(CancellationToken cancellationToken = default);
    Task<SemesterDetailsDto?> GetSemesterByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateSemesterAsync(CreateUpdateSemesterDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateSemesterAsync(int id, CreateUpdateSemesterDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteSemesterAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RoomDetailsDto>> GetAllRoomsAsync(CancellationToken cancellationToken = default);
    Task<RoomDetailsDto?> GetRoomByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateRoomAsync(CreateUpdateRoomDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateRoomAsync(int id, CreateUpdateRoomDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteRoomAsync(int id, CancellationToken cancellationToken = default);

    // Generic status table CRUD (tableName: CourseStatuses, StudentStatuses, InstructorStatuses, OfferingStatuses, EnrollmentStatuses, AttendanceStatuses)
    Task<IReadOnlyList<StatusLookupItemDto>> GetStatusItemsAsync(string tableName, CancellationToken cancellationToken = default);
    Task<StatusLookupItemDto?> GetStatusItemByIdAsync(string tableName, int id, CancellationToken cancellationToken = default);
    Task<int> CreateStatusItemAsync(string tableName, CreateUpdateStatusItemDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusItemAsync(string tableName, int id, CreateUpdateStatusItemDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteStatusItemAsync(string tableName, int id, CancellationToken cancellationToken = default);
}

