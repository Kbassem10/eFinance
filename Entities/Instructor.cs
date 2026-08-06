namespace StudentRegistrationPortal.Api.Entities;

public class Instructor
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public ICollection<Course> Courses { get; set; }
        = new List<Course>();
}