namespace StudentRegistrationPortal.Domain.Entities;


public class CourseOffering
{
    public int CourseOfferingId { get; set; }

    public int CourseId { get; set; }

    public Course? Course { get; set; }

    public int SemesterId { get; set; }

    public Semester? Semester { get; set; }

    public int OfferingStatusId { get; set; }

    public OfferingStatus? OfferingStatus { get; set; }

    public string SectionNumber { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public int CurrentEnrollmentCount { get; set; }

    public bool RegistrationOpen { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<CourseOfferingInstructor> CourseOfferingInstructors { get; set; } = new List<CourseOfferingInstructor>();

    public ICollection<CourseSchedule> CourseSchedules { get; set; } = new List<CourseSchedule>();

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();
}
