namespace StudentRegistrationPortal.Domain.Entities;

public class CourseStatus
{
    public int CourseStatusId { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
