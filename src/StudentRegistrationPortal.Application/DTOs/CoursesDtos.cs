using System.ComponentModel;

namespace StudentRegistrationPortal.Application.DTOs;

public record CourseScheduleDto
{
    public int CourseScheduleId { get; init; }
    public int CourseOfferingId { get; init; }
    public int RoomId { get; init; }
    public string? RoomNumber { get; init; }
    public string? BuildingName { get; init; }
    public int DayOfWeek { get; init; }
    public string DayName { get; init; } = string.Empty;
    public string StartTime { get; init; } = string.Empty;
    public string EndTime { get; init; } = string.Empty;
}

public record CourseOfferingInstructorDto
{
    public int InstructorId { get; init; }
    public string InstructorName { get; init; } = string.Empty;
    public bool IsPrimary { get; init; } = true;
}

public record CourseOfferingDetailsDto
{
    public int CourseOfferingId { get; init; }
    public int CourseId { get; init; }
    public int SemesterId { get; init; }
    public string SemesterName { get; init; } = string.Empty;
    public int OfferingStatusId { get; init; }
    public string OfferingStatusName { get; init; } = string.Empty;
    public string SectionNumber { get; init; } = "SEC-01";
    public int Capacity { get; init; } = 30;
    public IReadOnlyList<CourseOfferingInstructorDto> Instructors { get; init; } = Array.Empty<CourseOfferingInstructorDto>();
    public IReadOnlyList<CourseScheduleDto> Schedules { get; init; } = Array.Empty<CourseScheduleDto>();
}

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
    public string? InstructorName { get; init; }
    public IReadOnlyList<int> DepartmentIds { get; init; } = Array.Empty<int>();
    public IReadOnlyList<int> PrerequisiteCourseIds { get; init; } = Array.Empty<int>();
    public IReadOnlyList<string> PrerequisiteCourseCodes { get; init; } = Array.Empty<string>();
    public IReadOnlyList<CourseOfferingDetailsDto> Offerings { get; init; } = Array.Empty<CourseOfferingDetailsDto>();
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record CreateCourseScheduleDto
{
    [DefaultValue(1)]
    public int RoomId { get; init; } = 1;

    [DefaultValue(1)]
    public int DayOfWeek { get; init; } = 1; // 1 = Monday, 2 = Tuesday, etc.

    [DefaultValue("09:00:00")]
    public string StartTime { get; init; } = "09:00:00";

    [DefaultValue("10:30:00")]
    public string EndTime { get; init; } = "10:30:00";
}

public record CreateCourseOfferingDto
{
    [DefaultValue(1)]
    public int SemesterId { get; init; } = 1;

    [DefaultValue(1)]
    public int OfferingStatusId { get; init; } = 1;

    [DefaultValue("SEC-01")]
    public string SectionNumber { get; init; } = "SEC-01";

    [DefaultValue(30)]
    public int Capacity { get; init; } = 30;

    public List<int> InstructorIds { get; init; } = new();

    public List<CreateCourseScheduleDto> Schedules { get; init; } = new();
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

    public List<CreateCourseOfferingDto> Offerings { get; init; } = new();
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

    public List<CreateCourseOfferingDto> Offerings { get; init; } = new();
}
