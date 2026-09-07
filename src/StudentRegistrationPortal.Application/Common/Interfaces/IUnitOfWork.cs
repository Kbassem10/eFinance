namespace StudentRegistrationPortal.Application.Common.Interfaces;

/// Coordinates work of multiple repositories under a single transaction boundary.
public interface IUnitOfWork : IAsyncDisposable
{
    IStudentRepository Students { get; }
    IUserRepository Users { get; }
    ICoursesRepository Courses { get; }
    IAdminRepository Admin { get; }
    ILookupRepository Lookups { get; }

    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}

