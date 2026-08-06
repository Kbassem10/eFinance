using Microsoft.EntityFrameworkCore;
using StudentRegistrationPortal.Api.Entities;

namespace StudentRegistrationPortal.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }

    public DbSet<Instructor> Instructors { get; set; }

    public DbSet<Course> Courses { get; set; }

    public DbSet<Enrollment> Enrollments { get; set; }

    public DbSet<CourseSession> CourseSessions { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Student>(entity =>
    {
        entity.ToTable("Students");
        entity.HasIndex(student => student.Email).IsUnique();
        entity.Property(student => student.FullName).HasMaxLength(200).IsRequired();
        entity.Property(student => student.Email).HasMaxLength(200).IsRequired();
    });

    modelBuilder.Entity<Instructor>(entity =>
    {
        entity.ToTable("Instructors");
        entity.HasIndex(instructor => instructor.Email).IsUnique();
        entity.Property(instructor => instructor.FullName).HasMaxLength(200).IsRequired();
        entity.Property(instructor => instructor.Email).HasMaxLength(200).IsRequired();
        entity.Property(instructor => instructor.Department).HasMaxLength(100).IsRequired();
    });

    modelBuilder.Entity<Course>(entity =>
    {
        entity.ToTable("Courses");
        entity.HasIndex(course => course.Code).IsUnique();
        entity.Property(course => course.Name).HasMaxLength(200).IsRequired();
        entity.Property(course => course.Code).HasMaxLength(50).IsRequired();
        entity.HasOne(course => course.Instructor)
            .WithMany(instructor => instructor.Courses)
            .HasForeignKey(course => course.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);
    });

    modelBuilder.Entity<Enrollment>(entity =>
    {
        entity.ToTable("Enrollments");
        entity.HasOne(enrollment => enrollment.Student)
            .WithMany(student => student.Enrollments)
            .HasForeignKey(enrollment => enrollment.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(enrollment => enrollment.Course)
            .WithMany(course => course.Enrollments)
            .HasForeignKey(enrollment => enrollment.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(enrollment => new
        {
            enrollment.StudentId,
            enrollment.CourseId
        }).IsUnique();
    });

    modelBuilder.Entity<CourseSession>(entity =>
    {
        entity.ToTable("CourseSessions");
        entity.Property(session => session.Location).HasMaxLength(200).IsRequired();
        entity.HasOne(session => session.Course)
            .WithMany(course => course.Sessions)
            .HasForeignKey(session => session.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    });
}
}