namespace StudentRegistrationPortal.Domain.Entities;

public class Room
{
    public int RoomId { get; set; }

    public string BuildingName { get; set; } = string.Empty;

    public string RoomNumber { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public string? RoomType { get; set; }

    public bool IsAvailable { get; set; }

    public ICollection<CourseSchedule> CourseSchedules { get; set; } = new List<CourseSchedule>();

    public ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();
}
