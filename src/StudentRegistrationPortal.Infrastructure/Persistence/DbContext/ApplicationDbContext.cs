using Microsoft.EntityFrameworkCore;
using StudentRegistrationPortal.Domain.Entities;

namespace StudentRegistrationPortal.Infrastructure.Persistence.DbContext;

public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<StudentStatus> StudentStatuses => Set<StudentStatus>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<InstructorStatus> InstructorStatuses => Set<InstructorStatus>();
    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<CourseStatus> CourseStatuses => Set<CourseStatus>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseDepartment> CourseDepartments => Set<CourseDepartment>();
    public DbSet<CoursePrerequisite> CoursePrerequisites => Set<CoursePrerequisite>();
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<OfferingStatus> OfferingStatuses => Set<OfferingStatus>();
    public DbSet<CourseOffering> CourseOfferings => Set<CourseOffering>();
    public DbSet<CourseOfferingInstructor> CourseOfferingInstructors => Set<CourseOfferingInstructor>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<CourseSchedule> CourseSchedules => Set<CourseSchedule>();
    public DbSet<EnrollmentStatus> EnrollmentStatuses => Set<EnrollmentStatus>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Lecture> Lectures => Set<Lecture>();
    public DbSet<AttendanceStatus> AttendanceStatuses => Set<AttendanceStatus>();
    public DbSet<Attendance> AttendanceRecords => Set<Attendance>();
    public DbSet<StudentHold> StudentHolds => Set<StudentHold>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Users
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.UserId);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // 2. Roles
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(e => e.RoleId);
            entity.HasIndex(e => e.RoleName).IsUnique();
        });

        // 3. UserRoles
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.HasOne(e => e.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 4. Departments
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasKey(e => e.DepartmentId);
            entity.HasIndex(e => e.DepartmentCode).IsUnique();
        });

        // 5. StudentStatuses
        modelBuilder.Entity<StudentStatus>(entity =>
        {
            entity.ToTable("StudentStatuses");
            entity.HasKey(e => e.StudentStatusId);
            entity.HasIndex(e => e.StatusName).IsUnique();
        });

        // 6. Students
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students");
            entity.HasKey(e => e.StudentId);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasIndex(e => e.StudentNumber).IsUnique();
            entity.Property(e => e.GPA).HasPrecision(3, 2);

            entity.HasOne(e => e.User)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Department)
                .WithMany(d => d.Students)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.StudentStatus)
                .WithMany(s => s.Students)
                .HasForeignKey(e => e.StudentStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 7. InstructorStatuses
        modelBuilder.Entity<InstructorStatus>(entity =>
        {
            entity.ToTable("InstructorStatuses");
            entity.HasKey(e => e.InstructorStatusId);
            entity.HasIndex(e => e.StatusName).IsUnique();
        });

        // 8. Instructors
        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.ToTable("Instructors");
            entity.HasKey(e => e.InstructorId);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasIndex(e => e.EmployeeNumber).IsUnique();
            entity.Property(e => e.Salary).HasPrecision(12, 2);

            entity.HasOne(e => e.User)
                .WithOne(u => u.Instructor)
                .HasForeignKey<Instructor>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Department)
                .WithMany(d => d.Instructors)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.InstructorStatus)
                .WithMany(s => s.Instructors)
                .HasForeignKey(e => e.InstructorStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 9. CourseStatuses
        modelBuilder.Entity<CourseStatus>(entity =>
        {
            entity.ToTable("CourseStatuses");
            entity.HasKey(e => e.CourseStatusId);
            entity.HasIndex(e => e.StatusName).IsUnique();
        });

        // 10. Courses
        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses");
            entity.HasKey(e => e.CourseId);
            entity.HasIndex(e => e.CourseCode).IsUnique();

            entity.HasOne(e => e.CourseStatus)
                .WithMany(s => s.Courses)
                .HasForeignKey(e => e.CourseStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 11. CourseDepartments
        modelBuilder.Entity<CourseDepartment>(entity =>
        {
            entity.ToTable("CourseDepartments");
            entity.HasKey(e => new { e.CourseId, e.DepartmentId });

            entity.HasOne(e => e.Course)
                .WithMany(c => c.CourseDepartments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Department)
                .WithMany(d => d.CourseDepartments)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 12. CoursePrerequisites
        modelBuilder.Entity<CoursePrerequisite>(entity =>
        {
            entity.ToTable("CoursePrerequisites");
            entity.HasKey(e => new { e.CourseId, e.PrerequisiteCourseId });

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Prerequisites)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.PrerequisiteCourse)
                .WithMany(c => c.RequiredFor)
                .HasForeignKey(e => e.PrerequisiteCourseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 13. Semesters
        modelBuilder.Entity<Semester>(entity =>
        {
            entity.ToTable("Semesters");
            entity.HasKey(e => e.SemesterId);
        });

        // 14. OfferingStatuses
        modelBuilder.Entity<OfferingStatus>(entity =>
        {
            entity.ToTable("OfferingStatuses");
            entity.HasKey(e => e.OfferingStatusId);
            entity.HasIndex(e => e.StatusName).IsUnique();
        });

        // 15. CourseOfferings
        modelBuilder.Entity<CourseOffering>(entity =>
        {
            entity.ToTable("CourseOfferings");
            entity.HasKey(e => e.CourseOfferingId);

            entity.HasOne(e => e.Course)
                .WithMany(c => c.CourseOfferings)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Semester)
                .WithMany(s => s.CourseOfferings)
                .HasForeignKey(e => e.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.OfferingStatus)
                .WithMany(os => os.CourseOfferings)
                .HasForeignKey(e => e.OfferingStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 16. CourseOfferingInstructors
        modelBuilder.Entity<CourseOfferingInstructor>(entity =>
        {
            entity.ToTable("CourseOfferingInstructors");
            entity.HasKey(e => new { e.CourseOfferingId, e.InstructorId });

            entity.HasOne(e => e.CourseOffering)
                .WithMany(co => co.CourseOfferingInstructors)
                .HasForeignKey(e => e.CourseOfferingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Instructor)
                .WithMany(i => i.CourseOfferingInstructors)
                .HasForeignKey(e => e.InstructorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 17. Rooms
        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("Rooms");
            entity.HasKey(e => e.RoomId);
        });

        // 18. CourseSchedules
        modelBuilder.Entity<CourseSchedule>(entity =>
        {
            entity.ToTable("CourseSchedules");
            entity.HasKey(e => e.CourseScheduleId);

            entity.HasOne(e => e.CourseOffering)
                .WithMany(co => co.CourseSchedules)
                .HasForeignKey(e => e.CourseOfferingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Room)
                .WithMany(r => r.CourseSchedules)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 19. EnrollmentStatuses
        modelBuilder.Entity<EnrollmentStatus>(entity =>
        {
            entity.ToTable("EnrollmentStatuses");
            entity.HasKey(e => e.EnrollmentStatusId);
            entity.HasIndex(e => e.StatusName).IsUnique();
        });

        // 20. Enrollments
        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("Enrollments");
            entity.HasKey(e => e.EnrollmentId);
            entity.HasIndex(e => new { e.StudentId, e.CourseOfferingId }).IsUnique();
            entity.Property(e => e.CourseworkGrade).HasPrecision(5, 2);
            entity.Property(e => e.MidtermGrade).HasPrecision(5, 2);
            entity.Property(e => e.FinalExamGrade).HasPrecision(5, 2);
            entity.Property(e => e.TotalGrade).HasPrecision(5, 2);
            entity.Property(e => e.GradePoints).HasPrecision(3, 2);

            entity.HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CourseOffering)
                .WithMany(co => co.Enrollments)
                .HasForeignKey(e => e.CourseOfferingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.EnrollmentStatus)
                .WithMany(es => es.Enrollments)
                .HasForeignKey(e => e.EnrollmentStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 21. Lectures
        modelBuilder.Entity<Lecture>(entity =>
        {
            entity.ToTable("Lectures");
            entity.HasKey(e => e.LectureId);

            entity.HasOne(e => e.CourseOffering)
                .WithMany(co => co.Lectures)
                .HasForeignKey(e => e.CourseOfferingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 22. AttendanceStatuses
        modelBuilder.Entity<AttendanceStatus>(entity =>
        {
            entity.ToTable("AttendanceStatuses");
            entity.HasKey(e => e.AttendanceStatusId);
            entity.HasIndex(e => e.StatusName).IsUnique();
        });

        // 23. Attendance
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.ToTable("Attendance");
            entity.HasKey(e => e.AttendanceId);
            entity.HasIndex(e => new { e.LectureId, e.StudentId }).IsUnique();

            entity.HasOne(e => e.Lecture)
                .WithMany(l => l.AttendanceRecords)
                .HasForeignKey(e => e.LectureId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Student)
                .WithMany(s => s.AttendanceRecords)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AttendanceStatus)
                .WithMany(ats => ats.AttendanceRecords)
                .HasForeignKey(e => e.AttendanceStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 24. StudentHolds
        modelBuilder.Entity<StudentHold>(entity =>
        {
            entity.ToTable("StudentHolds");
            entity.HasKey(e => e.StudentHoldId);

            entity.HasOne(e => e.Student)
                .WithMany(s => s.StudentHolds)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
