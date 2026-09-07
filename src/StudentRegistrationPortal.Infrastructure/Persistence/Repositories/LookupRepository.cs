using Microsoft.Extensions.Logging;
using MySqlConnector;
using StudentRegistrationPortal.Application.Common.Interfaces;
using StudentRegistrationPortal.Application.DTOs;

namespace StudentRegistrationPortal.Infrastructure.Persistence.Repositories;

public class LookupRepository : ILookupRepository
{
    private readonly Func<Task<MySqlConnection>> _connectionProvider;
    private readonly Func<MySqlTransaction?> _transactionProvider;
    private readonly ILogger<LookupRepository> _logger;

    public LookupRepository(
        Func<Task<MySqlConnection>> connectionProvider,
        Func<MySqlTransaction?> transactionProvider,
        ILogger<LookupRepository> logger)
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

    public async Task<IReadOnlyList<LookupItemDto>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var list = new List<LookupItemDto>();
            const string sql = "SELECT DepartmentId, DepartmentName, DepartmentCode FROM Departments ORDER BY DepartmentName ASC;";
            await using var cmd = await CreateCommandAsync(sql);
            await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await r.ReadAsync(cancellationToken))
            {
                list.Add(new LookupItemDto(
                    r.GetInt32("DepartmentId"),
                    $"{r.GetString("DepartmentName")} ({r.GetString("DepartmentCode")})"
                ));
            }
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department lookups");
            throw;
        }
    }

    public async Task<IReadOnlyList<LookupItemDto>> GetSemestersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var list = new List<LookupItemDto>();
            const string sql = "SELECT SemesterId, SemesterName, AcademicYear FROM Semesters ORDER BY SemesterId ASC;";
            await using var cmd = await CreateCommandAsync(sql);
            await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await r.ReadAsync(cancellationToken))
            {
                list.Add(new LookupItemDto(
                    r.GetInt32("SemesterId"),
                    r.GetString("SemesterName")
                ));
            }
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving semester lookups");
            throw;
        }
    }

    public async Task<IReadOnlyList<LookupItemDto>> GetInstructorsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var list = new List<LookupItemDto>();
            const string sql = "SELECT InstructorId, FirstName, LastName, AcademicTitle FROM Instructors ORDER BY FirstName, LastName ASC;";
            await using var cmd = await CreateCommandAsync(sql);
            await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await r.ReadAsync(cancellationToken))
            {
                var title = r.IsDBNull(r.GetOrdinal("AcademicTitle")) ? "Faculty" : r.GetString("AcademicTitle");
                list.Add(new LookupItemDto(
                    r.GetInt32("InstructorId"),
                    $"{r.GetString("FirstName")} {r.GetString("LastName")} ({title})"
                ));
            }
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving instructor lookups");
            throw;
        }
    }

    public async Task<IReadOnlyList<LookupItemDto>> GetRoomsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var list = new List<LookupItemDto>();
            const string sql = "SELECT RoomId, BuildingName, RoomNumber, Capacity FROM Rooms ORDER BY BuildingName, RoomNumber ASC;";
            await using var cmd = await CreateCommandAsync(sql);
            await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await r.ReadAsync(cancellationToken))
            {
                list.Add(new LookupItemDto(
                    r.GetInt32("RoomId"),
                    $"{r.GetString("BuildingName")} - Room {r.GetString("RoomNumber")} (Cap: {r.GetInt32("Capacity")})"
                ));
            }
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving room lookups");
            throw;
        }
    }

    public async Task<IReadOnlyList<LookupItemDto>> GetCourseStatusesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var list = new List<LookupItemDto>();
            const string sql = "SELECT CourseStatusId, StatusName FROM CourseStatuses ORDER BY CourseStatusId ASC;";
            await using var cmd = await CreateCommandAsync(sql);
            await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await r.ReadAsync(cancellationToken))
            {
                list.Add(new LookupItemDto(
                    r.GetInt32("CourseStatusId"),
                    r.GetString("StatusName")
                ));
            }
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course status lookups");
            throw;
        }
    }
}
