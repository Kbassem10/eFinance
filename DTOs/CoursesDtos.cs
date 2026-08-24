using System.ComponentModel;

namespace StudentRegistrationPortal.Api.DTOs;

public record CourseDetailsDto
{
    public int CourseId { get; init; }
    public string CourseCode { get; init; } = string.Empty;
    public string CourseName { get; init; } = string.Empty;
    public int CreditHours { get; init; }
    public string DifficultyLevel { get; init; } = "Undergraduate";
    public int CourseStatusId { get; init; }
    public string StatusName { get; init; } = string.Empty;
    public string? AssignedDepartments { get; init; }
    public IReadOnlyList<int> DepartmentIds { get; init; } = Array.Empty<int>();
    public IReadOnlyList<int> PrerequisiteCourseIds { get; init; } = Array.Empty<int>();
    public IReadOnlyList<string> PrerequisiteCourseCodes { get; init; } = Array.Empty<string>();
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record CreateCourseDto
{
    [DefaultValue("CS101")]
    public string CourseCode { get; init; } = "CS101";

    [DefaultValue("Introduction to Computer Science")]
    public string CourseName { get; init; } = "Introduction to Computer Science";

    [DefaultValue(3)]
    public int CreditHours { get; init; } = 3;

    [DefaultValue("Beginner")]
    public string DifficultyLevel { get; init; } = "Beginner";

    [DefaultValue(1)]
    public int CourseStatusId { get; init; } = 1;

    public List<int> DepartmentIds { get; init; } = new();

    public List<int> PrerequisiteCourseIds { get; init; } = new();
}

public record UpdateCourseDto
{
    [DefaultValue("Introduction to Computer Science")]
    public string CourseName { get; init; } = "Introduction to Computer Science";

    [DefaultValue(3)]
    public int CreditHours { get; init; } = 3;

    [DefaultValue("Beginner")]
    public string DifficultyLevel { get; init; } = "Beginner";

    [DefaultValue(1)]
    public int CourseStatusId { get; init; } = 1;

    public List<int> DepartmentIds { get; init; } = new();

    public List<int> PrerequisiteCourseIds { get; init; } = new();
}
