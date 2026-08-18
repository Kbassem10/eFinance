using System.ComponentModel;

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
    [DefaultValue("karim.bassem@student.edu")]
    public string Email { get; init; } = "karim.bassem@student.edu";

    [DefaultValue("P@ssw0rd123!")]
    public string Password { get; init; } = "P@ssw0rd123!";

    [DefaultValue(1)]
    public int DepartmentId { get; init; } = 1;

    [DefaultValue(1)]
    public int StudentStatusId { get; init; } = 1;

    [DefaultValue("STU-2026-001")]
    public string StudentNumber { get; init; } = "STU-2026-001";

    [DefaultValue("Karim")]
    public string FirstName { get; init; } = "Karim";

    [DefaultValue("Bassem")]
    public string? MiddleName { get; init; } = "Bassem";

    [DefaultValue("Joseph")]
    public string LastName { get; init; } = "Joseph";

    [DefaultValue("30001011234567")]
    public string? NationalId { get; init; } = "30001011234567";

    public DateOnly DateOfBirth { get; init; } = new DateOnly(2005, 12, 10);

    [DefaultValue("Male")]
    public string? Gender { get; init; } = "Male";

    [DefaultValue("+201204331187")]
    public string? PhoneNumber { get; init; } = "+201204331187";

    [DefaultValue("Smart Village, Giza, Egypt")]
    public string? Address { get; init; } = "Smart Village, Giza, Egypt";

    public DateOnly AdmissionDate { get; init; } = new DateOnly(2026, 9, 1);
}

public record UpdateStudentDto
{
    [DefaultValue(1)]
    public int DepartmentId { get; init; } = 1;

    [DefaultValue(1)]
    public int StudentStatusId { get; init; } = 1;

    [DefaultValue("Karim")]
    public string FirstName { get; init; } = "Karim";

    [DefaultValue("Bassem")]
    public string? MiddleName { get; init; } = "Bassem";

    [DefaultValue("Joseph")]
    public string LastName { get; init; } = "Joseph";

    [DefaultValue("+201204331187")]
    public string? PhoneNumber { get; init; } = "+201204331187";

    [DefaultValue("Smart Village, Giza, Egypt")]
    public string? Address { get; init; } = "Smart Village, Giza, Egypt";

    public DateOnly AdmissionDate { get; init; } = new DateOnly(2026, 9, 1);
}

public record LoginRequestDto
{
    [DefaultValue("karim.bassem@student.edu")]
    public string Email { get; init; } = "karim.bassem@student.edu";

    [DefaultValue("P@ssw0rd123!")]
    public string Password { get; init; } = "P@ssw0rd123!";
}

public record LoginResponseDto(
    string Token,
    string TokenType,
    DateTime ExpiresAt,
    StudentDetailsDto Student
);
