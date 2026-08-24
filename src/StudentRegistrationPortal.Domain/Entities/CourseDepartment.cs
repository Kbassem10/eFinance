namespace StudentRegistrationPortal.Domain.Entities;

public class CourseDepartment
{
    public int CourseId { get; set; }

    public Course? Course { get; set; }

    public int DepartmentId { get; set; }

    public Department? Department { get; set; }

    public bool IsPrimaryDepartment { get; set; }
}
