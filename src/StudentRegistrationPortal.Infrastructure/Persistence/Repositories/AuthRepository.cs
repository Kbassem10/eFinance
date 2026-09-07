using System.Data;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using StudentRegistrationPortal.Application.Common.Interfaces;
using StudentRegistrationPortal.Application.DTOs;
using StudentRegistrationPortal.Domain.Entities;

namespace StudentRegistrationPortal.Infrastructure.Persistence.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly Func<Task<MySqlConnection>> _connectionProvider;
    private readonly Func<MySqlTransaction?> _transactionProvider;
    private readonly ILogger<AuthRepository> _logger;

    public AuthRepository(
        Func<Task<MySqlConnection>> connectionProvider,
        Func<MySqlTransaction?> transactionProvider,
        ILogger<AuthRepository> logger)
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

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
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
            _logger.LogError(ex, "Error retrieving user by email '{Email}' in AuthRepository", email);
            throw;
        }
    }

    public async Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
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
            _logger.LogError(ex, "Error retrieving user by ID {UserId} in AuthRepository", userId);
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
            _logger.LogError(ex, "Error retrieving roles for user ID {UserId} in AuthRepository", userId);
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
            _logger.LogError(ex, "Error retrieving role IDs for user ID {UserId} in AuthRepository", userId);
            throw;
        }
    }

    public async Task<StudentDetailsDto?> GetStudentByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = @"
                SELECT 
                    s.StudentId, s.StudentNumber, s.FirstName, s.MiddleName, s.LastName,
                    s.NationalId, s.DateOfBirth, s.Gender, s.PhoneNumber, s.Address,
                    s.AdmissionDate, s.AcademicLevel, s.GPA, s.CompletedCreditHours,
                    s.DepartmentId, d.DepartmentName, d.DepartmentCode,
                    s.StudentStatusId, ss.StatusName,
                    u.Email, s.CreatedAt, s.UpdatedAt
                FROM Students s
                INNER JOIN Users u ON s.UserId = u.UserId
                LEFT JOIN Departments d ON s.DepartmentId = d.DepartmentId
                LEFT JOIN StudentStatuses ss ON s.StudentStatusId = ss.StudentStatusId
                WHERE s.UserId = @UserId
                LIMIT 1;";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                var firstName = reader.GetString("FirstName");
                var middleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? null : reader.GetString("MiddleName");
                var lastName = reader.GetString("LastName");
                var fullName = string.IsNullOrWhiteSpace(middleName)
                    ? $"{firstName} {lastName}"
                    : $"{firstName} {middleName} {lastName}";

                return new StudentDetailsDto
                {
                    StudentId = reader.GetInt32("StudentId"),
                    StudentNumber = reader.GetString("StudentNumber"),
                    FirstName = firstName,
                    MiddleName = middleName,
                    LastName = lastName,
                    FullName = fullName,
                    Email = reader.GetString("Email"),
                    NationalId = reader.IsDBNull(reader.GetOrdinal("NationalId")) ? null : reader.GetString("NationalId"),
                    DateOfBirth = DateOnly.FromDateTime(reader.GetDateTime("DateOfBirth")),
                    Gender = reader.IsDBNull(reader.GetOrdinal("Gender")) ? null : reader.GetString("Gender"),
                    PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString("PhoneNumber"),
                    Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString("Address"),
                    AdmissionDate = DateOnly.FromDateTime(reader.GetDateTime("AdmissionDate")),
                    AcademicLevel = reader.GetInt32("AcademicLevel"),
                    GPA = reader.GetDecimal("GPA"),
                    CompletedCreditHours = reader.GetInt32("CompletedCreditHours"),
                    DepartmentId = reader.GetInt32("DepartmentId"),
                    DepartmentName = reader.IsDBNull(reader.GetOrdinal("DepartmentName")) ? string.Empty : reader.GetString("DepartmentName"),
                    DepartmentCode = reader.IsDBNull(reader.GetOrdinal("DepartmentCode")) ? string.Empty : reader.GetString("DepartmentCode"),
                    StudentStatusId = reader.GetInt32("StudentStatusId"),
                    StatusName = reader.IsDBNull(reader.GetOrdinal("StatusName")) ? string.Empty : reader.GetString("StatusName"),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student by user ID {UserId} in AuthRepository", userId);
            throw;
        }
    }

    public async Task<UserDetailsDto?> GetUserDetailsByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await GetUserByIdAsync(userId, cancellationToken);
            if (user == null) return null;

            var roles = await GetUserRolesAsync(userId, cancellationToken);
            var roleNames = roles.Select(r => r.RoleName).ToList();
            var student = await GetStudentByUserIdAsync(userId, cancellationToken);

            return new UserDetailsDto
            {
                UserId = user.UserId,
                Email = user.Email,
                IsActive = user.IsActive,
                Roles = roleNames,
                StudentId = student?.StudentId,
                StudentNumber = student?.StudentNumber,
                FullName = student?.FullName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user details for ID {UserId} in AuthRepository", userId);
            throw;
        }
    }
}
