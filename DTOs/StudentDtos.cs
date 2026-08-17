namespace StudentRegistrationPortal.Api.DTOs;

public record StudentDetailsDto
{
    public int StudentId { get; init; }
    public string StudentNumber { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string? MiddleName { get; init; }
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? NationalId { get; init; }
    public DateOnly DateOfBirth { get; init; }
    public string? Gender { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Address { get; init; }
    public DateOnly AdmissionDate { get; init; }
    public int AcademicLevel { get; init; }
    public decimal GPA { get; init; }
    public int CompletedCreditHours { get; init; }
    public int DepartmentId { get; init; }
    public string DepartmentName { get; init; } = string.Empty;
    public string DepartmentCode { get; init; } = string.Empty;
    public int StudentStatusId { get; init; }
    public string StatusName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record CreateStudentDto
{
    public int UserId { get; init; }
    public int DepartmentId { get; init; }
    public int StudentStatusId { get; init; } = 1; // Default: Active / Enrolled
    public string StudentNumber { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string NationalId { get; init; } = string.Empty;
    public DateOnly AdmissionDate { get; init; } = DateOnly.FromDateTime(DateTime.UtcNow);
}

public record UpdateStudentDto
{
    public int DepartmentId { get; init; }
    public int StudentStatusId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateOnly AdmissionDate { get; init; }
}
