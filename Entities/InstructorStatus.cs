namespace StudentRegistrationPortal.Api.Entities;

public class InstructorStatus
{
    public int InstructorStatusId { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();
}
