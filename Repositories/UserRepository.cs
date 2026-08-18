using System.Data;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using StudentRegistrationPortal.Api.Entities;

namespace StudentRegistrationPortal.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly Func<Task<MySqlConnection>> _connectionProvider;
    private readonly Func<MySqlTransaction?> _transactionProvider;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(
        Func<Task<MySqlConnection>> connectionProvider,
        Func<MySqlTransaction?> transactionProvider,
        ILogger<UserRepository> logger)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        _transactionProvider = transactionProvider ?? throw new ArgumentNullException(nameof(transactionProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private async Task<MySqlCommand> CreateCommandAsync(string sql)
    {
        var connection = await _connectionProvider();
        var command = new MySqlCommand(sql, connection);

        if (_transactionProvider() is { } transaction)
        {
            command.Transaction = transaction;
        }

        return command;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        try
        {
            const string sql = @"
                SELECT UserId, Email, PasswordHash, IsActive, CreatedAt, UpdatedAt
                FROM Users
                WHERE Email = @Email
                LIMIT 1;";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@Email", email.Trim().ToLowerInvariant());

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                return new User
                {
                    UserId = reader.GetInt32("UserId"),
                    Email = reader.GetString("Email"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by email '{Email}'", email);
            throw;
        }
    }

    public async Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = @"
                SELECT UserId, Email, PasswordHash, IsActive, CreatedAt, UpdatedAt
                FROM Users
                WHERE UserId = @UserId
                LIMIT 1;";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                return new User
                {
                    UserId = reader.GetInt32("UserId"),
                    Email = reader.GetString("Email"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by ID {UserId}", userId);
            throw;
        }
    }

    public async Task<int> CreateAsync(string email, string passwordHash, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        try
        {
            const string sql = @"
                INSERT INTO Users (Email, PasswordHash, IsActive, CreatedAt, UpdatedAt)
                VALUES (@Email, @PasswordHash, 1, UTC_TIMESTAMP(), UTC_TIMESTAMP());
                SELECT LAST_INSERT_ID();";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@Email", email.Trim().ToLowerInvariant());
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);

            var result = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt32(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email '{Email}'", email);
            throw;
        }
    }

    public async Task AssignRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = @"
                INSERT INTO UserRoles (UserId, RoleId, AssignedAt)
                VALUES (@UserId, @RoleId, UTC_TIMESTAMP())
                ON DUPLICATE KEY UPDATE AssignedAt = AssignedAt;";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@RoleId", roleId);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role ID {RoleId} to user ID {UserId}", roleId, userId);
            throw;
        }
    }
}
