using System.Data;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using StudentRegistrationPortal.Application.Common.Interfaces;
using StudentRegistrationPortal.Application.DTOs;
using StudentRegistrationPortal.Domain.Entities;

namespace StudentRegistrationPortal.Infrastructure.Persistence.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly Func<Task<MySqlConnection>> _connectionProvider;
    private readonly Func<MySqlTransaction?> _transactionProvider;
    private readonly ILogger<AdminRepository> _logger;

    public AdminRepository(
        Func<Task<MySqlConnection>> connectionProvider,
        Func<MySqlTransaction?> transactionProvider,
        ILogger<AdminRepository> logger)
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

    public async Task<IReadOnlyList<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
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
            _logger.LogError(ex, "Error retrieving all users for admin");
            throw;
        }
    }

    public async Task<IReadOnlyList<AdminEnrollmentDetailsDto>> GetAllEnrollmentsAsync(int? statusId = null, CancellationToken cancellationToken = default)
    {
        var list = new List<AdminEnrollmentDetailsDto>();
        try
        {
            const string sql = @"
                SELECT 
                    e.EnrollmentId,
                    e.StudentId,
                    s.StudentNumber,
                    CONCAT(s.FirstName, ' ', s.LastName) AS StudentName,
                    u.Email AS StudentEmail,
                    c.CourseId,
                    c.CourseCode,
                    c.CourseName,
                    c.CreditHours,
                    GROUP_CONCAT(DISTINCT CONCAT(i.FirstName, ' ', i.LastName) SEPARATOR ', ') AS InstructorName,
                    e.EnrollmentStatusId,
                    es.StatusName,
                    e.RegistrationDate
                FROM Enrollments e
                INNER JOIN Students s ON e.StudentId = s.StudentId
                INNER JOIN Users u ON s.UserId = u.UserId
                INNER JOIN CourseOfferings co ON e.CourseOfferingId = co.CourseOfferingId
                INNER JOIN Courses c ON co.CourseId = c.CourseId
                INNER JOIN EnrollmentStatuses es ON e.EnrollmentStatusId = es.EnrollmentStatusId
                LEFT JOIN CourseOfferingInstructors coi ON co.CourseOfferingId = coi.CourseOfferingId
                LEFT JOIN Instructors i ON coi.InstructorId = i.InstructorId
                WHERE (@StatusId IS NULL OR e.EnrollmentStatusId = @StatusId)
                GROUP BY e.EnrollmentId, e.StudentId, s.StudentNumber, s.FirstName, s.LastName, u.Email, c.CourseId, c.CourseCode, c.CourseName, c.CreditHours, e.EnrollmentStatusId, es.StatusName, e.RegistrationDate
                ORDER BY e.EnrollmentId DESC;";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@StatusId", (object?)statusId ?? DBNull.Value);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                list.Add(new AdminEnrollmentDetailsDto
                {
                    EnrollmentId = reader.GetInt32("EnrollmentId"),
                    StudentId = reader.GetInt32("StudentId"),
                    StudentNumber = reader.GetString("StudentNumber"),
                    StudentName = reader.GetString("StudentName"),
                    StudentEmail = reader.GetString("StudentEmail"),
                    CourseId = reader.GetInt32("CourseId"),
                    CourseCode = reader.GetString("CourseCode"),
                    CourseName = reader.GetString("CourseName"),
                    CreditHours = reader.GetInt32("CreditHours"),
                    InstructorName = reader.IsDBNull(reader.GetOrdinal("InstructorName")) ? null : reader.GetString("InstructorName"),
                    EnrollmentStatusId = reader.GetInt32("EnrollmentStatusId"),
                    StatusName = reader.GetString("StatusName"),
                    RegistrationDate = reader.GetDateTime("RegistrationDate")
                });
            }

            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all enrollments for admin");
            throw;
        }
    }

    public async Task<bool> UpdateEnrollmentStatusAsync(int enrollmentId, int statusId, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = @"
                UPDATE Enrollments
                SET EnrollmentStatusId = @StatusId
                WHERE EnrollmentId = @EnrollmentId;";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@EnrollmentId", enrollmentId);
            command.Parameters.AddWithValue("@StatusId", statusId);

            int rows = await command.ExecuteNonQueryAsync(cancellationToken);
            return rows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating enrollment ID {EnrollmentId} to status {StatusId}", enrollmentId, statusId);
            throw;
        }
    }
}
