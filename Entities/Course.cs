namespace StudentRegistrationPortal.Api.Entities;

public class Course
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public int CreditHours { get; set; }

    public int InstructorId { get; set; }

    public Instructor? Instructor { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; }
        = new List<Enrollment>();

    public ICollection<CourseSession> Sessions { get; set; }
        = new List<CourseSession>();
}