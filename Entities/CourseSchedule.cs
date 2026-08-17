namespace StudentRegistrationPortal.Api.Entities;

public class CourseSchedule
{
    public int CourseScheduleId { get; set; }

    public int CourseOfferingId { get; set; }

    public CourseOffering? CourseOffering { get; set; }

    public int RoomId { get; set; }

    public Room? Room { get; set; }

    public string DayOfWeek { get; set; } = string.Empty;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? ScheduleType { get; set; }
}
