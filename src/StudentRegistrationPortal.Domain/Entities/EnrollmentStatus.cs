namespace StudentRegistrationPortal.Domain.Entities;

public class EnrollmentStatus
{
    public int EnrollmentStatusId { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
