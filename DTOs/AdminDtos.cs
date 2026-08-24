using System.ComponentModel;

namespace StudentRegistrationPortal.Api.DTOs;

public record AdminDetailsDto
{
    public int UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record AdminLoginResponseDto(
    string Token,
    string TokenType,
    DateTime ExpiresAt,
    AdminDetailsDto Admin
);

public record AdminEnrollmentDetailsDto
{
    public int EnrollmentId { get; init; }
    public int StudentId { get; init; }
    public string StudentNumber { get; init; } = string.Empty;
    public string StudentName { get; init; } = string.Empty;
    public string StudentEmail { get; init; } = string.Empty;
    public int CourseId { get; init; }
    public string CourseCode { get; init; } = string.Empty;
    public string CourseName { get; init; } = string.Empty;
    public int CreditHours { get; init; }
    public string? InstructorName { get; init; }
    public int EnrollmentStatusId { get; init; }
    public string StatusName { get; init; } = string.Empty;
    public DateTime RegistrationDate { get; init; }
}

public record UpdateEnrollmentStatusDto
{
    [DefaultValue(1)]
    public int EnrollmentStatusId { get; init; } = 1;
}

public record LookupItemDto(int Id, string Name, string? Extra = null);

public record AdminLookupsDto
{
    public IReadOnlyList<LookupItemDto> Departments { get; init; } = Array.Empty<LookupItemDto>();
    public IReadOnlyList<LookupItemDto> Semesters { get; init; } = Array.Empty<LookupItemDto>();
    public IReadOnlyList<LookupItemDto> Instructors { get; init; } = Array.Empty<LookupItemDto>();
    public IReadOnlyList<LookupItemDto> Rooms { get; init; } = Array.Empty<LookupItemDto>();
    public IReadOnlyList<LookupItemDto> CourseStatuses { get; init; } = Array.Empty<LookupItemDto>();
}
