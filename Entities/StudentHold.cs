namespace StudentRegistrationPortal.Api.Entities;

public class StudentHold
{
    public int StudentHoldId { get; set; }

    public int StudentId { get; set; }

    public Student? Student { get; set; }

    public string HoldType { get; set; } = string.Empty;

    public string? Reason { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}
