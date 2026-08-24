namespace StudentRegistrationPortal.Domain.Entities;

public class Instructor
{
    public int InstructorId { get; set; }

    public int UserId { get; set; }

    public User? User { get; set; }

    public int DepartmentId { get; set; }

    public Department? Department { get; set; }

    public int InstructorStatusId { get; set; }

    public InstructorStatus? InstructorStatus { get; set; }

    public string EmployeeNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = string.Empty;

    public string? NationalId { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string? PhoneNumber { get; set; }

    public string? AcademicTitle { get; set; }

    public DateOnly HireDate { get; set; }

    public decimal Salary { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<CourseOfferingInstructor> CourseOfferingInstructors { get; set; } = new List<CourseOfferingInstructor>();
}
