namespace StudentRegistrationPortal.Domain.Entities;

public class Course
{
    public int CourseId { get; set; }

    public int CourseStatusId { get; set; }

    public CourseStatus? CourseStatus { get; set; }

    public string CourseCode { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int CreditHours { get; set; }

    public int MaximumStudents { get; set; }

    public string? DifficultyLevel { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<CourseDepartment> CourseDepartments { get; set; } = new List<CourseDepartment>();

    public ICollection<CourseOffering> CourseOfferings { get; set; } = new List<CourseOffering>();

    // Courses that THIS course requires as prerequisites.
    public ICollection<CoursePrerequisite> Prerequisites { get; set; } = new List<CoursePrerequisite>();

    // Courses for which THIS course is listed as a prerequisite.
    public ICollection<CoursePrerequisite> RequiredFor { get; set; } = new List<CoursePrerequisite>();
}
