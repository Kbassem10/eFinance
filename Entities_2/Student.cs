namespace StudentRegistrationPortal.Api.Entities;

public class Student
{
    public int StudentId { get; set; }

    public int UserId { get; set; }

    public User? User { get; set; }

    public int DepartmentId { get; set; }

    public Department? Department { get; set; }

    public int StudentStatusId { get; set; }

    public StudentStatus? StudentStatus { get; set; }

    public string StudentNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = string.Empty;

    public string? NationalId { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public DateOnly AdmissionDate { get; set; }

    public int AcademicLevel { get; set; }

    public decimal GPA { get; set; }

    public int CompletedCreditHours { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public ICollection<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();

    public ICollection<StudentHold> StudentHolds { get; set; } = new List<StudentHold>();
}
