namespace StudentRegistrationPortal.Api.Entities;

public class Semester
{
    public int SemesterId { get; set; }

    public string SemesterName { get; set; } = string.Empty;

    public string AcademicYear { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public DateTime RegistrationStartDate { get; set; }

    public DateTime RegistrationEndDate { get; set; }

    public bool IsCurrent { get; set; }

    public ICollection<CourseOffering> CourseOfferings { get; set; } = new List<CourseOffering>();
}
