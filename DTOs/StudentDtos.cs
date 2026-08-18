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
    public string Email { get; init; } = "karim.bassem@student.edu";
    public string Password { get; init; } = "P@ssw0rd123!";
    public int DepartmentId { get; init; } = 1;
    public int StudentStatusId { get; init; } = 1;
    public string StudentNumber { get; init; } = "STU-2026-001";
    public string FirstName { get; init; } = "Karim";
    public string? MiddleName { get; init; } = "Bassem";
    public string LastName { get; init; } = "Joseph";
    public string? NationalId { get; init; } = "30001011234567";
    public DateOnly DateOfBirth { get; init; } = new DateOnly(2002, 5, 15);
    public string? Gender { get; init; } = "Male";
    public string? PhoneNumber { get; init; } = "+201204331187";
    public string? Address { get; init; } = "Smart Village, Giza, Egypt";
    public DateOnly AdmissionDate { get; init; } = new DateOnly(2026, 9, 1);
}

public record UpdateStudentDto
{
    public int DepartmentId { get; init; } = 1;
    public int StudentStatusId { get; init; } = 1;
    public string FirstName { get; init; } = "Karim";
    public string? MiddleName { get; init; } = "Bassem";
    public string LastName { get; init; } = "Joseph";
    public string? PhoneNumber { get; init; } = "+201204331187";
    public string? Address { get; init; } = "Smart Village, Giza, Egypt";
    public DateOnly AdmissionDate { get; init; } = new DateOnly(2026, 9, 1);
}
