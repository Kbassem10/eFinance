using StudentRegistrationPortal.Api.Entities;

namespace StudentRegistrationPortal.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(string email, string passwordHash, CancellationToken cancellationToken = default);
    Task AssignRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default);
}
