using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "User ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "A valid User ID must be greater than 0.")]
    [DefaultValue(2)]
    public int UserId { get; init; } = 2;

    [Required(ErrorMessage = "Department ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "A valid Department ID must be greater than 0.")]
    [DefaultValue(1)]
    public int DepartmentId { get; init; } = 1;

    [Required(ErrorMessage = "Student Status ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "A valid Status ID must be greater than 0.")]
    [DefaultValue(1)]
    public int StudentStatusId { get; init; } = 1;

    [Required(ErrorMessage = "Student Number is required.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Student Number must be between 3 and 30 characters.")]
    [DefaultValue("STU-2026-001")]
    public string StudentNumber { get; init; } = "STU-2026-001";

    [Required(ErrorMessage = "First Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 100 characters.")]
    [DefaultValue("Karim")]
    public string FirstName { get; init; } = "Karim";

    [StringLength(100, ErrorMessage = "Middle Name cannot exceed 100 characters.")]
    [DefaultValue("Bassem")]
    public string? MiddleName { get; init; } = "Bassem";

    [Required(ErrorMessage = "Last Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 100 characters.")]
    [DefaultValue("Joseph")]
    public string LastName { get; init; } = "Joseph";

    [StringLength(30, ErrorMessage = "National ID cannot exceed 30 characters.")]
    [DefaultValue("30001011234567")]
    public string? NationalId { get; init; } = "30001011234567";

    [Required(ErrorMessage = "Date of Birth is required.")]
    public DateOnly DateOfBirth { get; init; } = new DateOnly(2002, 5, 15);

    [StringLength(20, ErrorMessage = "Gender cannot exceed 20 characters.")]
    [DefaultValue("Male")]
    public string? Gender { get; init; } = "Male";

    [Phone(ErrorMessage = "Invalid phone number format.")]
    [StringLength(30, ErrorMessage = "Phone Number cannot exceed 30 characters.")]
    [DefaultValue("+201204331187")]
    public string? PhoneNumber { get; init; } = "+201204331187";

    [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters.")]
    [DefaultValue("Smart Village, Giza, Egypt")]
    public string? Address { get; init; } = "Smart Village, Giza, Egypt";

    [Required(ErrorMessage = "Admission Date is required.")]
    public DateOnly AdmissionDate { get; init; } = new DateOnly(2026, 9, 1);
}

public record UpdateStudentDto
{
    [Required(ErrorMessage = "Department ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "A valid Department ID must be greater than 0.")]
    [DefaultValue(1)]
    public int DepartmentId { get; init; } = 1;

    [Required(ErrorMessage = "Student Status ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "A valid Status ID must be greater than 0.")]
    [DefaultValue(1)]
    public int StudentStatusId { get; init; } = 1;

    [Required(ErrorMessage = "First Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 100 characters.")]
    [DefaultValue("Karim")]
    public string FirstName { get; init; } = "Karim";

    [StringLength(100, ErrorMessage = "Middle Name cannot exceed 100 characters.")]
    [DefaultValue("Bassem")]
    public string? MiddleName { get; init; } = "Bassem";

    [Required(ErrorMessage = "Last Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 100 characters.")]
    [DefaultValue("Joseph")]
    public string LastName { get; init; } = "Joseph";

    [Phone(ErrorMessage = "Invalid phone number format.")]
    [StringLength(30, ErrorMessage = "Phone Number cannot exceed 30 characters.")]
    [DefaultValue("+201204331187")]
    public string? PhoneNumber { get; init; } = "+201204331187";

    [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters.")]
    [DefaultValue("Smart Village, Giza, Egypt")]
    public string? Address { get; init; } = "Smart Village, Giza, Egypt";

    [Required(ErrorMessage = "Admission Date is required.")]
    public DateOnly AdmissionDate { get; init; } = new DateOnly(2026, 9, 1);
}
