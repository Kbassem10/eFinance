using System.Data;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using StudentRegistrationPortal.Api.DTOs;

namespace StudentRegistrationPortal.Api.Repositories;

/// Student Repository executing standard SQL queries using the shared Unit of Work connection and transaction.
public class StudentRepository : IStudentRepository
{
    private readonly Func<Task<MySqlConnection>> _connectionProvider;
    private readonly Func<MySqlTransaction?> _transactionProvider;
    private readonly ILogger<StudentRepository> _logger;

    public StudentRepository(
        Func<Task<MySqlConnection>> connectionProvider,
        Func<MySqlTransaction?> transactionProvider,
        ILogger<StudentRepository> logger)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        _transactionProvider = transactionProvider ?? throw new ArgumentNullException(nameof(transactionProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// Creates a MySqlCommand attached to the shared connection and active transaction
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

    public async Task<IReadOnlyList<StudentDetailsDto>> GetAllAsync()
    {
        var list = new List<StudentDetailsDto>();

        try
        {
            const string sql = @"
                SELECT 
                    StudentId, StudentNumber, FirstName, MiddleName, LastName, FullName,
                    Email, NationalId, DateOfBirth, Gender, PhoneNumber, Address,
                    AdmissionDate, AcademicLevel, GPA, CompletedCreditHours,
                    DepartmentId, DepartmentName, DepartmentCode,
                    StudentStatusId, StatusName, CreatedAt, UpdatedAt
                FROM vw_Students
                ORDER BY StudentId DESC;";

            await using var command = await CreateCommandAsync(sql);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(MapStudentFromReader(reader));
            }

            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving students list");
            throw;
        }
    }

    public async Task<StudentDetailsDto?> GetByIdAsync(int studentId)
    {
        try
        {
            const string sql = @"
                SELECT 
                    StudentId, StudentNumber, FirstName, MiddleName, LastName, FullName,
                    Email, NationalId, DateOfBirth, Gender, PhoneNumber, Address,
                    AdmissionDate, AcademicLevel, GPA, CompletedCreditHours,
                    DepartmentId, DepartmentName, DepartmentCode,
                    StudentStatusId, StatusName, CreatedAt, UpdatedAt
                FROM vw_Students
                WHERE StudentId = @StudentId
                LIMIT 1;";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@StudentId", studentId);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapStudentFromReader(reader);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student ID {StudentId}", studentId);
            throw;
        }
    }

    public async Task<StudentDetailsDto?> GetByStudentNumberAsync(string studentNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(studentNumber);

        try
        {
            const string sql = @"
                SELECT 
                    StudentId, StudentNumber, FirstName, MiddleName, LastName, FullName,
                    Email, NationalId, DateOfBirth, Gender, PhoneNumber, Address,
                    AdmissionDate, AcademicLevel, GPA, CompletedCreditHours,
                    DepartmentId, DepartmentName, DepartmentCode,
                    StudentStatusId, StatusName, CreatedAt, UpdatedAt
                FROM vw_Students
                WHERE StudentNumber = @StudentNumber
                LIMIT 1;";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@StudentNumber", studentNumber);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapStudentFromReader(reader);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student number '{StudentNumber}'", studentNumber);
            throw;
        }
    }

    public async Task<int> CreateAsync(int userId, CreateStudentDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        try
        {
            const string sql = @"
                INSERT INTO Students (
                    UserId, DepartmentId, StudentStatusId, StudentNumber,
                    FirstName, MiddleName, LastName, NationalId, DateOfBirth,
                    Gender, PhoneNumber, Address, AdmissionDate,
                    AcademicLevel, GPA, CompletedCreditHours, CreatedAt, UpdatedAt
                ) VALUES (
                    @UserId, @DepartmentId, @StudentStatusId, @StudentNumber,
                    @FirstName, @MiddleName, @LastName, @NationalId, @DateOfBirth,
                    @Gender, @PhoneNumber, @Address, @AdmissionDate,
                    1, 0.00, 0, NOW(), NOW()
                );
                SELECT LAST_INSERT_ID();";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@DepartmentId", dto.DepartmentId);
            command.Parameters.AddWithValue("@StudentStatusId", dto.StudentStatusId);
            command.Parameters.AddWithValue("@StudentNumber", dto.StudentNumber);
            command.Parameters.AddWithValue("@FirstName", dto.FirstName);
            command.Parameters.AddWithValue("@MiddleName", (object?)dto.MiddleName ?? DBNull.Value);
            command.Parameters.AddWithValue("@LastName", dto.LastName);
            command.Parameters.AddWithValue("@NationalId", (object?)dto.NationalId ?? DBNull.Value);
            command.Parameters.AddWithValue("@DateOfBirth", dto.DateOfBirth.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@Gender", (object?)dto.Gender ?? DBNull.Value);
            command.Parameters.AddWithValue("@PhoneNumber", (object?)dto.PhoneNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@Address", (object?)dto.Address ?? DBNull.Value);
            command.Parameters.AddWithValue("@AdmissionDate", dto.AdmissionDate.ToDateTime(TimeOnly.MinValue));

            var result = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt32(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating student");
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int studentId, UpdateStudentDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        try
        {
            const string sql = @"
                UPDATE Students
                SET DepartmentId = @DepartmentId,
                    StudentStatusId = @StudentStatusId,
                    FirstName = @FirstName,
                    MiddleName = @MiddleName,
                    LastName = @LastName,
                    PhoneNumber = @PhoneNumber,
                    Address = @Address,
                    AdmissionDate = @AdmissionDate,
                    UpdatedAt = NOW()
                WHERE StudentId = @StudentId;";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@StudentId", studentId);
            command.Parameters.AddWithValue("@DepartmentId", dto.DepartmentId);
            command.Parameters.AddWithValue("@StudentStatusId", dto.StudentStatusId);
            command.Parameters.AddWithValue("@FirstName", dto.FirstName);
            command.Parameters.AddWithValue("@MiddleName", (object?)dto.MiddleName ?? DBNull.Value);
            command.Parameters.AddWithValue("@LastName", dto.LastName);
            command.Parameters.AddWithValue("@PhoneNumber", (object?)dto.PhoneNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@Address", (object?)dto.Address ?? DBNull.Value);
            command.Parameters.AddWithValue("@AdmissionDate", dto.AdmissionDate.ToDateTime(TimeOnly.MinValue));

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating student ID {StudentId}", studentId);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int studentId)
    {
        try
        {
            const string sql = "DELETE FROM Students WHERE StudentId = @StudentId;";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@StudentId", studentId);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting student ID {StudentId}", studentId);
            throw;
        }
    }

    public async Task<int> GetTotalCreditHoursAsync(int studentId, int semesterId)
    {
        try
        {
            const string sql = "SELECT fn_GetStudentTotalCreditHours(@StudentId, @SemesterId);";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@StudentId", studentId);
            command.Parameters.AddWithValue("@SemesterId", semesterId);

            var result = await command.ExecuteScalarAsync();
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting credit hours for student {StudentId}, semester {SemesterId}", studentId, semesterId);
            throw;
        }
    }

    private static StudentDetailsDto MapStudentFromReader(MySqlDataReader reader)
    {
        return new StudentDetailsDto
        {
            StudentId = reader.IsDBNull(reader.GetOrdinal("StudentId")) ? 0 : reader.GetInt32("StudentId"),
            StudentNumber = reader.IsDBNull(reader.GetOrdinal("StudentNumber")) ? string.Empty : reader.GetString("StudentNumber"),
            FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? string.Empty : reader.GetString("FirstName"),
            MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? null : reader.GetString("MiddleName"),
            LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? string.Empty : reader.GetString("LastName"),
            FullName = reader.IsDBNull(reader.GetOrdinal("FullName")) ? string.Empty : reader.GetString("FullName"),
            Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString("Email"),
            NationalId = reader.IsDBNull(reader.GetOrdinal("NationalId")) ? null : reader.GetString("NationalId"),
            DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? DateOnly.MinValue : DateOnly.FromDateTime(reader.GetDateTime("DateOfBirth")),
            Gender = reader.IsDBNull(reader.GetOrdinal("Gender")) ? null : reader.GetString("Gender"),
            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString("PhoneNumber"),
            Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString("Address"),
            AdmissionDate = reader.IsDBNull(reader.GetOrdinal("AdmissionDate")) ? DateOnly.MinValue : DateOnly.FromDateTime(reader.GetDateTime("AdmissionDate")),
            AcademicLevel = reader.IsDBNull(reader.GetOrdinal("AcademicLevel")) ? 0 : reader.GetInt32("AcademicLevel"),
            GPA = reader.IsDBNull(reader.GetOrdinal("GPA")) ? 0.00m : reader.GetDecimal("GPA"),
            CompletedCreditHours = reader.IsDBNull(reader.GetOrdinal("CompletedCreditHours")) ? 0 : reader.GetInt32("CompletedCreditHours"),
            DepartmentId = reader.IsDBNull(reader.GetOrdinal("DepartmentId")) ? 0 : reader.GetInt32("DepartmentId"),
            DepartmentName = reader.IsDBNull(reader.GetOrdinal("DepartmentName")) ? string.Empty : reader.GetString("DepartmentName"),
            DepartmentCode = reader.IsDBNull(reader.GetOrdinal("DepartmentCode")) ? string.Empty : reader.GetString("DepartmentCode"),
            StudentStatusId = reader.IsDBNull(reader.GetOrdinal("StudentStatusId")) ? 0 : reader.GetInt32("StudentStatusId"),
            StatusName = reader.IsDBNull(reader.GetOrdinal("StatusName")) ? string.Empty : reader.GetString("StatusName"),
            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.MinValue : reader.GetDateTime("CreatedAt"),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime("UpdatedAt")
        };
    }
}
