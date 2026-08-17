namespace StudentRegistrationPortal.Api.Repositories;

/// <summary>
/// Coordinates work of multiple repositories under a single transaction boundary.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    IStudentRepository Students { get; }

    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
