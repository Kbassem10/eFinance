namespace StudentRegistrationPortal.Domain.Entities;

public class AttendanceStatus
{
    public int AttendanceStatusId { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public ICollection<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
}
