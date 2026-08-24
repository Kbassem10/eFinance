namespace StudentRegistrationPortal.Domain.Entities;

public class Department
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = string.Empty;

    public string DepartmentCode { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();

    public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();

    public ICollection<CourseDepartment> CourseDepartments { get; set; } = new List<CourseDepartment>();
}
