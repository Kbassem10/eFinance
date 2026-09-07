using System.ComponentModel;

namespace StudentRegistrationPortal.Application.DTOs;




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

