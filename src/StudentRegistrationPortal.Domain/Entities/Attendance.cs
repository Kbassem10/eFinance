namespace StudentRegistrationPortal.Domain.Entities;

public class Attendance
{
    public int AttendanceId { get; set; }

    public int LectureId { get; set; }

    public Lecture? Lecture { get; set; }

    public int StudentId { get; set; }

    public Student? Student { get; set; }

    public int AttendanceStatusId { get; set; }

    public AttendanceStatus? AttendanceStatus { get; set; }

    public DateTime? CheckInTime { get; set; }

    public string? Notes { get; set; }

    public DateTime RecordedAt { get; set; }
}
