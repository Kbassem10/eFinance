namespace StudentRegistrationPortal.Api.Entities;

public class CoursePrerequisite
{
    public int CourseId { get; set; }

    public Course? Course { get; set; }

    public int PrerequisiteCourseId { get; set; }

    public Course? PrerequisiteCourse { get; set; }

    public string? MinimumGrade { get; set; }
}
