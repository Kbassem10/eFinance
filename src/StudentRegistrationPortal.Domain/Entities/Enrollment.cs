namespace StudentRegistrationPortal.Domain.Entities;

public class Enrollment
{
    public int EnrollmentId { get; set; }

    public int StudentId { get; set; }

    public Student? Student { get; set; }

    public int CourseOfferingId { get; set; }

    public CourseOffering? CourseOffering { get; set; }

    public int EnrollmentStatusId { get; set; }

    public EnrollmentStatus? EnrollmentStatus { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public DateTime? DropDate { get; set; }

    public decimal? CourseworkGrade { get; set; }

    public decimal? MidtermGrade { get; set; }

    public decimal? FinalExamGrade { get; set; }

    public decimal? TotalGrade { get; set; }

    public string? LetterGrade { get; set; }

    public decimal? GradePoints { get; set; }

    public bool IsPassed { get; set; }
}
