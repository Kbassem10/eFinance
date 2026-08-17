namespace StudentRegistrationPortal.Api.Entities;

public class OfferingStatus
{
    public int OfferingStatusId { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public ICollection<CourseOffering> CourseOfferings { get; set; } = new List<CourseOffering>();
}
