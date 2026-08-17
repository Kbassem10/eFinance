namespace StudentRegistrationPortal.Api.Entities;

public class StudentStatus
{
    public int StudentStatusId { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public ICollection<Student> Students { get; set; } = new List<Student>();
}
