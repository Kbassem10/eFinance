namespace StudentRegistrationPortal.Api.Entities;

public class Lecture
{
    public int LectureId { get; set; }

    public int CourseOfferingId { get; set; }

    public CourseOffering? CourseOffering { get; set; }

    public int RoomId { get; set; }

    public Room? Room { get; set; }

    public string LectureTitle { get; set; } = string.Empty;

    public string? LectureTopic { get; set; }

    public DateOnly LectureDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsCancelled { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
}
