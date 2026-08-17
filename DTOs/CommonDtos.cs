namespace StudentRegistrationPortal.Api.DTOs;

public record UserDetailsDto
{
    public int UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string Roles { get; init; } = string.Empty;
    public string ProfileType { get; init; } = string.Empty;
    public string ProfileName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record CourseDetailsDto
{
    public int CourseId { get; init; }
    public string CourseCode { get; init; } = string.Empty;
    public string CourseName { get; init; } = string.Empty;
    public int CreditHours { get; init; }
    public string DifficultyLevel { get; init; } = string.Empty;
    public string StatusName { get; init; } = string.Empty;
    public string AssignedDepartments { get; init; } = string.Empty;
}

public record EnrollmentDetailsDto
{
    public int EnrollmentId { get; init; }
    public int StudentId { get; init; }
    public string StudentNumber { get; init; } = string.Empty;
    public string StudentName { get; init; } = string.Empty;
    public string CourseCode { get; init; } = string.Empty;
    public string CourseName { get; init; } = string.Empty;
    public string SectionNumber { get; init; } = string.Empty;
    public string SemesterName { get; init; } = string.Empty;
    public int AcademicYear { get; init; }
    public string EnrollmentStatus { get; init; } = string.Empty;
    public decimal? TotalGrade { get; init; }
    public string? LetterGrade { get; init; }
    public decimal? GradePoints { get; init; }
}
