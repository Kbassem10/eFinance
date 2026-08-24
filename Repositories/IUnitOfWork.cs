namespace StudentRegistrationPortal.Api.Repositories;

/// Coordinates work of multiple repositories under a single transaction boundary.
public interface IUnitOfWork : IAsyncDisposable
{
    IStudentRepository Students { get; }
    IUserRepository Users { get; }
    ICoursesRepository Courses { get; }

    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
