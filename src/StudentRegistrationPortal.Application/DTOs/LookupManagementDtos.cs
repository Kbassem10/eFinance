namespace StudentRegistrationPortal.Application.DTOs;

public record DepartmentDetailsDto(
    int DepartmentId,
    string DepartmentCode,
    string DepartmentName,
    DateTime CreatedAt
);

public record CreateUpdateDepartmentDto(
    string DepartmentCode,
    string DepartmentName
);

public record SemesterDetailsDto(
    int SemesterId,
    string SemesterName,
    string AcademicYear,
    DateOnly StartDate,
    DateOnly EndDate,
    bool IsCurrent
);

public record CreateUpdateSemesterDto(
    string SemesterName,
    string AcademicYear,
    DateOnly StartDate,
    DateOnly EndDate,
    bool IsCurrent = false
);

public record RoomDetailsDto(
    int RoomId,
    string BuildingName,
    string RoomNumber,
    int Capacity
);

public record CreateUpdateRoomDto(
    string BuildingName,
    string RoomNumber,
    int Capacity
);


public record StatusLookupItemDto(
    int Id,
    string StatusName,
    string? Description = null
);

public record CreateUpdateStatusItemDto(
    string StatusName,
    string? Description = null
);
