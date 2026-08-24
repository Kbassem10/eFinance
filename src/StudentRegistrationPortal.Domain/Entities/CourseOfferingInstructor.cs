namespace StudentRegistrationPortal.Domain.Entities;

public class CourseOfferingInstructor
{
    public int CourseOfferingId { get; set; }

    public CourseOffering? CourseOffering { get; set; }

    public int InstructorId { get; set; }

    public Instructor? Instructor { get; set; }

    public bool IsPrimaryInstructor { get; set; }

    public DateTime AssignedAt { get; set; }
}
