using Microsoft.EntityFrameworkCore;
using StudentRegistrationPortal.Api.Entities;

namespace StudentRegistrationPortal.Api.Data;

public class ApplicationDbContext2 : DbContext
{
    public ApplicationDbContext2(DbContextOptions<ApplicationDbContext2> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    public DbSet<StudentStatus> StudentStatuses { get; set; }
    public DbSet<InstructorStatus> InstructorStatuses { get; set; }
    public DbSet<CourseStatus> CourseStatuses { get; set; }
    public DbSet<OfferingStatus> OfferingStatuses { get; set; }
    public DbSet<EnrollmentStatus> EnrollmentStatuses { get; set; }
    public DbSet<AttendanceStatus> AttendanceStatuses { get; set; }

    public DbSet<Department> Departments { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Semester> Semesters { get; set; }

    public DbSet<Course> Courses { get; set; }
    public DbSet<CourseDepartment> CourseDepartments { get; set; }
    public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }

    public DbSet<Room> Rooms { get; set; }
    public DbSet<CourseOffering> CourseOfferings { get; set; }
    public DbSet<CourseOfferingInstructor> CourseOfferingInstructors { get; set; }
    public DbSet<CourseSchedule> CourseSchedules { get; set; }

    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Lecture> Lectures { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<StudentHold> StudentHolds { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- Users / Roles ----------
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.UserId);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).HasMaxLength(255).IsRequired();
            entity.Property(u => u.PasswordHash).HasMaxLength(500).IsRequired();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(r => r.RoleId);
            entity.HasIndex(r => r.RoleName).IsUnique();
            entity.Property(r => r.RoleName).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- Lookup / status tables ----------
        modelBuilder.Entity<StudentStatus>(entity =>
        {
            entity.ToTable("StudentStatuses");
            entity.HasKey(s => s.StudentStatusId);
            entity.Property(s => s.StatusName).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<InstructorStatus>(entity =>
        {
            entity.ToTable("InstructorStatuses");
            entity.HasKey(s => s.InstructorStatusId);
            entity.Property(s => s.StatusName).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<CourseStatus>(entity =>
        {
            entity.ToTable("CourseStatuses");
            entity.HasKey(s => s.CourseStatusId);
            entity.Property(s => s.StatusName).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<OfferingStatus>(entity =>
        {
            entity.ToTable("OfferingStatuses");
            entity.HasKey(s => s.OfferingStatusId);
            entity.Property(s => s.StatusName).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<EnrollmentStatus>(entity =>
        {
            entity.ToTable("EnrollmentStatuses");
            entity.HasKey(s => s.EnrollmentStatusId);
            entity.Property(s => s.StatusName).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<AttendanceStatus>(entity =>
        {
            entity.ToTable("AttendanceStatuses");
            entity.HasKey(s => s.AttendanceStatusId);
            entity.Property(s => s.StatusName).HasMaxLength(50).IsRequired();
        });

        // ---------- Departments ----------
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasKey(d => d.DepartmentId);
            entity.HasIndex(d => d.DepartmentCode).IsUnique();
            entity.Property(d => d.DepartmentName).HasMaxLength(150).IsRequired();
            entity.Property(d => d.DepartmentCode).HasMaxLength(20).IsRequired();
            entity.Property(d => d.Description).HasMaxLength(500);
        });

        // ---------- Students ----------
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students");
            entity.HasKey(s => s.StudentId);
            entity.HasIndex(s => s.UserId).IsUnique();
            entity.HasIndex(s => s.StudentNumber).IsUnique();
            entity.Property(s => s.StudentNumber).HasMaxLength(30).IsRequired();
            entity.Property(s => s.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(s => s.MiddleName).HasMaxLength(100);
            entity.Property(s => s.LastName).HasMaxLength(100).IsRequired();
            entity.Property(s => s.NationalId).HasMaxLength(30);
            entity.Property(s => s.Gender).HasMaxLength(20);
            entity.Property(s => s.PhoneNumber).HasMaxLength(30);
            entity.Property(s => s.Address).HasMaxLength(500);
            entity.Property(s => s.GPA).HasColumnType("decimal(4,2)");

            entity.HasOne(s => s.User)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.Department)
                .WithMany(d => d.Students)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.StudentStatus)
                .WithMany(st => st.Students)
                .HasForeignKey(s => s.StudentStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- Instructors ----------
        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.ToTable("Instructors");
            entity.HasKey(i => i.InstructorId);
            entity.HasIndex(i => i.UserId).IsUnique();
            entity.HasIndex(i => i.EmployeeNumber).IsUnique();
            entity.Property(i => i.EmployeeNumber).HasMaxLength(30).IsRequired();
            entity.Property(i => i.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(i => i.MiddleName).HasMaxLength(100);
            entity.Property(i => i.LastName).HasMaxLength(100).IsRequired();
            entity.Property(i => i.NationalId).HasMaxLength(30);
            entity.Property(i => i.PhoneNumber).HasMaxLength(30);
            entity.Property(i => i.AcademicTitle).HasMaxLength(100);
            entity.Property(i => i.Salary).HasColumnType("decimal(12,2)");

            entity.HasOne(i => i.User)
                .WithOne(u => u.Instructor)
                .HasForeignKey<Instructor>(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(i => i.Department)
                .WithMany(d => d.Instructors)
                .HasForeignKey(i => i.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(i => i.InstructorStatus)
                .WithMany(st => st.Instructors)
                .HasForeignKey(i => i.InstructorStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- Semesters ----------
        modelBuilder.Entity<Semester>(entity =>
        {
            entity.ToTable("Semesters");
            entity.HasKey(s => s.SemesterId);
            entity.Property(s => s.SemesterName).HasMaxLength(100).IsRequired();
            entity.Property(s => s.AcademicYear).HasMaxLength(20).IsRequired();
        });

        // ---------- Courses ----------
        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses");
            entity.HasKey(c => c.CourseId);
            entity.HasIndex(c => c.CourseCode).IsUnique();
            entity.Property(c => c.CourseCode).HasMaxLength(30).IsRequired();
            entity.Property(c => c.CourseName).HasMaxLength(200).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(1000);
            entity.Property(c => c.DifficultyLevel).HasMaxLength(30);

            entity.HasOne(c => c.CourseStatus)
                .WithMany(cs => cs.Courses)
                .HasForeignKey(c => c.CourseStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CourseDepartment>(entity =>
        {
            entity.ToTable("CourseDepartments");
            entity.HasKey(cd => new { cd.CourseId, cd.DepartmentId });

            entity.HasOne(cd => cd.Course)
                .WithMany(c => c.CourseDepartments)
                .HasForeignKey(cd => cd.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cd => cd.Department)
                .WithMany(d => d.CourseDepartments)
                .HasForeignKey(cd => cd.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CoursePrerequisite>(entity =>
        {
            entity.ToTable("CoursePrerequisites");
            entity.HasKey(cp => new { cp.CourseId, cp.PrerequisiteCourseId });
            entity.Property(cp => cp.MinimumGrade).HasMaxLength(5);

            // Self-referencing many-to-many on Course: both sides must be
            // Restrict, since SQL Server won't allow cascading self-references.
            entity.HasOne(cp => cp.Course)
                .WithMany(c => c.Prerequisites)
                .HasForeignKey(cp => cp.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(cp => cp.PrerequisiteCourse)
                .WithMany(c => c.RequiredFor)
                .HasForeignKey(cp => cp.PrerequisiteCourseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- Rooms ----------
        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("Rooms");
            entity.HasKey(r => r.RoomId);
            entity.HasIndex(r => new { r.BuildingName, r.RoomNumber }).IsUnique();
            entity.Property(r => r.BuildingName).HasMaxLength(150).IsRequired();
            entity.Property(r => r.RoomNumber).HasMaxLength(30).IsRequired();
            entity.Property(r => r.RoomType).HasMaxLength(50);
        });

        // ---------- Course offerings ----------
        modelBuilder.Entity<CourseOffering>(entity =>
        {
            entity.ToTable("CourseOfferings");
            entity.HasKey(co => co.CourseOfferingId);
            entity.Property(co => co.SectionNumber).HasMaxLength(20).IsRequired();
            entity.HasIndex(co => new { co.CourseId, co.SemesterId, co.SectionNumber }).IsUnique();

            entity.HasOne(co => co.Course)
                .WithMany(c => c.CourseOfferings)
                .HasForeignKey(co => co.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(co => co.Semester)
                .WithMany(s => s.CourseOfferings)
                .HasForeignKey(co => co.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(co => co.OfferingStatus)
                .WithMany(os => os.CourseOfferings)
                .HasForeignKey(co => co.OfferingStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CourseOfferingInstructor>(entity =>
        {
            entity.ToTable("CourseOfferingInstructors");
            entity.HasKey(coi => new { coi.CourseOfferingId, coi.InstructorId });

            entity.HasOne(coi => coi.CourseOffering)
                .WithMany(co => co.CourseOfferingInstructors)
                .HasForeignKey(coi => coi.CourseOfferingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(coi => coi.Instructor)
                .WithMany(i => i.CourseOfferingInstructors)
                .HasForeignKey(coi => coi.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CourseSchedule>(entity =>
        {
            entity.ToTable("CourseSchedules");
            entity.HasKey(cs => cs.CourseScheduleId);
            entity.Property(cs => cs.DayOfWeek).HasMaxLength(20).IsRequired();
            entity.Property(cs => cs.ScheduleType).HasMaxLength(30);

            entity.HasOne(cs => cs.CourseOffering)
                .WithMany(co => co.CourseSchedules)
                .HasForeignKey(cs => cs.CourseOfferingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cs => cs.Room)
                .WithMany(r => r.CourseSchedules)
                .HasForeignKey(cs => cs.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- Enrollments ----------
        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("Enrollments");
            entity.HasKey(e => e.EnrollmentId);
            entity.HasIndex(e => new { e.StudentId, e.CourseOfferingId }).IsUnique();
            entity.Property(e => e.LetterGrade).HasMaxLength(5);
            entity.Property(e => e.CourseworkGrade).HasColumnType("decimal(5,2)");
            entity.Property(e => e.MidtermGrade).HasColumnType("decimal(5,2)");
            entity.Property(e => e.FinalExamGrade).HasColumnType("decimal(5,2)");
            entity.Property(e => e.TotalGrade).HasColumnType("decimal(5,2)");
            entity.Property(e => e.GradePoints).HasColumnType("decimal(4,2)");

            entity.HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CourseOffering)
                .WithMany(co => co.Enrollments)
                .HasForeignKey(e => e.CourseOfferingId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.EnrollmentStatus)
                .WithMany(es => es.Enrollments)
                .HasForeignKey(e => e.EnrollmentStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- Lectures / Attendance ----------
        modelBuilder.Entity<Lecture>(entity =>
        {
            entity.ToTable("Lectures");
            entity.HasKey(l => l.LectureId);
            entity.Property(l => l.LectureTitle).HasMaxLength(200).IsRequired();
            entity.Property(l => l.LectureTopic).HasMaxLength(500);

            entity.HasOne(l => l.CourseOffering)
                .WithMany(co => co.Lectures)
                .HasForeignKey(l => l.CourseOfferingId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(l => l.Room)
                .WithMany(r => r.Lectures)
                .HasForeignKey(l => l.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.ToTable("Attendance");
            entity.HasKey(a => a.AttendanceId);
            entity.HasIndex(a => new { a.LectureId, a.StudentId }).IsUnique();
            entity.Property(a => a.Notes).HasMaxLength(500);

            entity.HasOne(a => a.Lecture)
                .WithMany(l => l.AttendanceRecords)
                .HasForeignKey(a => a.LectureId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.Student)
                .WithMany(s => s.AttendanceRecords)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.AttendanceStatus)
                .WithMany(status => status.AttendanceRecords)
                .HasForeignKey(a => a.AttendanceStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- Student holds ----------
        modelBuilder.Entity<StudentHold>(entity =>
        {
            entity.ToTable("StudentHolds");
            entity.HasKey(sh => sh.StudentHoldId);
            entity.Property(sh => sh.HoldType).HasMaxLength(100).IsRequired();
            entity.Property(sh => sh.Reason).HasMaxLength(500);

            entity.HasOne(sh => sh.Student)
                .WithMany(s => s.StudentHolds)
                .HasForeignKey(sh => sh.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
