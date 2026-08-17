using System.Data;
using MySqlConnector;
using StudentRegistrationPortal.Api.Data;
using StudentRegistrationPortal.Api.DTOs;

namespace StudentRegistrationPortal.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ISqlDataAccess _db;

    public UserRepository(ISqlDataAccess db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<IReadOnlyList<UserDetailsDto>> GetAllUserDetailsAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT UserId, Email, IsActive, Roles, ProfileType, ProfileName, CreatedAt, UpdatedAt
            FROM vw_UserDetails
            ORDER BY UserId ASC;";

        return await _db.QueryAsync(sql, MapUserDetailsDto, cancellationToken: cancellationToken);
    }

    public async Task<UserDetailsDto?> GetUserDetailsByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT UserId, Email, IsActive, Roles, ProfileType, ProfileName, CreatedAt, UpdatedAt
            FROM vw_UserDetails
            WHERE UserId = @UserId
            LIMIT 1;";

        var parameters = new[] { new MySqlParameter("@UserId", userId) };
        return await _db.QueryFirstOrDefaultAsync(sql, MapUserDetailsDto, parameters, cancellationToken: cancellationToken);
    }

    public async Task<UserDetailsDto?> GetUserDetailsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT UserId, Email, IsActive, Roles, ProfileType, ProfileName, CreatedAt, UpdatedAt
            FROM vw_UserDetails
            WHERE Email = @Email
            LIMIT 1;";

        var parameters = new[] { new MySqlParameter("@Email", email) };
        return await _db.QueryFirstOrDefaultAsync(sql, MapUserDetailsDto, parameters, cancellationToken: cancellationToken);
    }

    private static UserDetailsDto MapUserDetailsDto(MySqlDataReader reader)
    {
        return new UserDetailsDto
        {
            UserId = reader.GetSafeInt32("UserId"),
            Email = reader.GetSafeString("Email"),
            IsActive = reader.GetSafeBool("IsActive"),
            Roles = reader.GetSafeString("Roles"),
            ProfileType = reader.GetSafeString("ProfileType"),
            ProfileName = reader.GetSafeString("ProfileName"),
            CreatedAt = reader.GetSafeDateTime("CreatedAt"),
            UpdatedAt = reader.GetSafeDateTime("UpdatedAt")
        };
    }
}
