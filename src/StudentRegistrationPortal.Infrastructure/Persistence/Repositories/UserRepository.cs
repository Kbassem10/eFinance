using StudentRegistrationPortal.Application.Common.Interfaces;
using System.Data;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using StudentRegistrationPortal.Application.DTOs;
using StudentRegistrationPortal.Domain.Entities;

namespace StudentRegistrationPortal.Infrastructure.Persistence.Repositories;

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

    public async Task<IReadOnlyList<Role>> GetUserRolesAsync(int userId, CancellationToken cancellationToken = default)
    {
        var roles = new List<Role>();
        try
        {
            const string sql = @"
                SELECT r.RoleId, r.RoleName
                FROM UserRoles ur
                INNER JOIN Roles r ON ur.RoleId = r.RoleId
                WHERE ur.UserId = @UserId;";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                roles.Add(new Role
                {
                    RoleId = reader.GetInt32("RoleId"),
                    RoleName = reader.GetString("RoleName")
                });
            }

            return roles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles for user ID {UserId}", userId);
            throw;
        }
    }

    public async Task<IReadOnlyList<int>> GetUserRoleIdsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var roleIds = new List<int>();
        try
        {
            const string sql = "SELECT RoleId FROM UserRoles WHERE UserId = @UserId;";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                roleIds.Add(reader.GetInt32("RoleId"));
            }

            return roleIds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role IDs for user ID {UserId}", userId);
            throw;
        }
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<User>();
        try
        {
            const string sql = @"
                SELECT UserId, Email, PasswordHash, IsActive, CreatedAt, UpdatedAt
                FROM Users
                ORDER BY UserId ASC;";

            await using var command = await CreateCommandAsync(sql);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                list.Add(new User
                {
                    UserId = reader.GetInt32("UserId"),
                    Email = reader.GetString("Email"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                });
            }

            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw;
        }
    }

    public async Task<AdminDetailsDto?> GetUserDetailsByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = @"
                SELECT UserId, Email, PasswordHash, IsActive, CreatedAt, UpdatedAt
                FROM Users
                WHERE UserId = @UserId
                LIMIT 1;";

            AdminDetailsDto? user = null;
            await using (var command = await CreateCommandAsync(sql))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                if (await reader.ReadAsync(cancellationToken))
                {
                    user = new AdminDetailsDto
                    {
                        UserId = reader.GetInt32("UserId"),
                        Email = reader.GetString("Email"),
                        IsActive = reader.GetBoolean("IsActive"),
                        CreatedAt = reader.GetDateTime("CreatedAt"),
                        UpdatedAt = reader.GetDateTime("UpdatedAt")
                    };
                }
            }

            if (user == null) return null;

            var roles = await GetUserRolesAsync(userId, cancellationToken);
            return user with { Roles = roles.Select(r => r.RoleName).ToList() };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user details for user ID {UserId}", userId);
            throw;
        }
    }

    public async Task<AdminLookupsDto> GetLookupsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var depts = new List<LookupItemDto>();
            const string deptSql = "SELECT DepartmentId, DepartmentName, DepartmentCode FROM Departments ORDER BY DepartmentName ASC;";
            await using (var cmd = await CreateCommandAsync(deptSql))
            {
                await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
                while (await r.ReadAsync(cancellationToken))
                {
                    depts.Add(new LookupItemDto(
                        r.GetInt32("DepartmentId"),
                        $"{r.GetString("DepartmentName")} ({r.GetString("DepartmentCode")})"
                    ));
                }
            }

            var semesters = new List<LookupItemDto>();
            const string semSql = "SELECT SemesterId, SemesterName, AcademicYear FROM Semesters ORDER BY SemesterId ASC;";
            await using (var cmd = await CreateCommandAsync(semSql))
            {
                await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
                while (await r.ReadAsync(cancellationToken))
                {
                    semesters.Add(new LookupItemDto(
                        r.GetInt32("SemesterId"),
                        r.GetString("SemesterName")
                    ));
                }
            }

            var instructors = new List<LookupItemDto>();
            const string instSql = "SELECT InstructorId, FirstName, LastName, AcademicTitle FROM Instructors ORDER BY FirstName, LastName ASC;";
            await using (var cmd = await CreateCommandAsync(instSql))
            {
                await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
                while (await r.ReadAsync(cancellationToken))
                {
                    var title = r.IsDBNull("AcademicTitle") ? "Faculty" : r.GetString("AcademicTitle");
                    instructors.Add(new LookupItemDto(
                        r.GetInt32("InstructorId"),
                        $"{r.GetString("FirstName")} {r.GetString("LastName")} ({title})"
                    ));
                }
            }

            var rooms = new List<LookupItemDto>();
            const string roomSql = "SELECT RoomId, BuildingName, RoomNumber, Capacity FROM Rooms ORDER BY BuildingName, RoomNumber ASC;";
            await using (var cmd = await CreateCommandAsync(roomSql))
            {
                await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
                while (await r.ReadAsync(cancellationToken))
                {
                    rooms.Add(new LookupItemDto(
                        r.GetInt32("RoomId"),
                        $"{r.GetString("BuildingName")} - Room {r.GetString("RoomNumber")} (Cap: {r.GetInt32("Capacity")})"
                    ));
                }
            }

            var statuses = new List<LookupItemDto>();
            const string statusSql = "SELECT CourseStatusId, StatusName FROM CourseStatuses ORDER BY CourseStatusId ASC;";
            await using (var cmd = await CreateCommandAsync(statusSql))
            {
                await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
                while (await r.ReadAsync(cancellationToken))
                {
                    statuses.Add(new LookupItemDto(
                        r.GetInt32("CourseStatusId"),
                        r.GetString("StatusName")
                    ));
                }
            }

            return new AdminLookupsDto
            {
                Departments = depts,
                Semesters = semesters,
                Instructors = instructors,
                Rooms = rooms,
                CourseStatuses = statuses
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving database lookups");
            throw;
        }
    }
}
