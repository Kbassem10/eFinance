using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using StudentRegistrationPortal.Application.Common.Interfaces;
using StudentRegistrationPortal.Application.DTOs;
using StudentRegistrationPortal.Domain.Entities;
using StudentRegistrationPortal.Infrastructure.Persistence.DbContext;

namespace StudentRegistrationPortal.Infrastructure.Persistence.Repositories;

public class LookupRepository : ILookupRepository
{
    private readonly Func<Task<MySqlConnection>> _connectionProvider;
    private readonly Func<MySqlTransaction?> _transactionProvider;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<LookupRepository> _logger;

    public LookupRepository(
        Func<Task<MySqlConnection>> connectionProvider,
        Func<MySqlTransaction?> transactionProvider,
        ApplicationDbContext dbContext,
        ILogger<LookupRepository> logger)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        _transactionProvider = transactionProvider ?? throw new ArgumentNullException(nameof(transactionProvider));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
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

    public async Task<IReadOnlyList<LookupItemDto>> GetDepartmentsAsync(string departmentCode = "", CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<Department> query = _dbContext.Departments.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(departmentCode) && !departmentCode.Equals("ALL", StringComparison.OrdinalIgnoreCase))
            {
                var filter = departmentCode.Trim();
                query = query.Where(d => d.DepartmentCode == filter || d.DepartmentName == filter || EF.Functions.Like(d.DepartmentName, $"%{filter}%"));
            }

            return await query
                .OrderBy(d => d.DepartmentName)
                .Select(d => new LookupItemDto(
                    d.DepartmentId,
                    d.DepartmentName
                ))
                .ToListAsync(cancellationToken);
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

    public async Task<IReadOnlyList<DepartmentDetailsDto>> GetAllDepartmentsAsync(string departmentCode = "", CancellationToken cancellationToken = default)
    {
        IQueryable<Department> query = _dbContext.Departments.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(departmentCode) && !departmentCode.Equals("ALL", StringComparison.OrdinalIgnoreCase))
        {
            var filter = departmentCode.Trim();
            query = query.Where(d => d.DepartmentCode == filter || d.DepartmentName == filter || EF.Functions.Like(d.DepartmentName, $"%{filter}%") || EF.Functions.Like(d.DepartmentCode, $"%{filter}%"));
        }

        return await query
            .OrderBy(d => d.DepartmentName)
            .Select(d => new DepartmentDetailsDto(
                d.DepartmentId,
                d.DepartmentCode,
                d.DepartmentName,
                d.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<DepartmentDetailsDto?> GetDepartmentByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Departments
            .AsNoTracking()
            .Where(d => d.DepartmentId == id)
            .Select(d => new DepartmentDetailsDto(
                d.DepartmentId,
                d.DepartmentCode,
                d.DepartmentName,
                d.CreatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CreateDepartmentAsync(CreateUpdateDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        var department = new Department
        {
            DepartmentCode = dto.DepartmentCode,
            DepartmentName = dto.DepartmentName,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Departments.Add(department);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return department.DepartmentId;
    }

    public async Task<bool> UpdateDepartmentAsync(int id, CreateUpdateDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        var department = await _dbContext.Departments
            .FirstOrDefaultAsync(d => d.DepartmentId == id, cancellationToken);

        if (department == null)
            return false;

        department.DepartmentCode = dto.DepartmentCode;
        department.DepartmentName = dto.DepartmentName;

        int rows = await _dbContext.SaveChangesAsync(cancellationToken);
        return rows > 0;
    }

    public async Task<bool> DeleteDepartmentAsync(int id, CancellationToken cancellationToken = default)
    {
        var department = await _dbContext.Departments
            .FirstOrDefaultAsync(d => d.DepartmentId == id, cancellationToken);

        if (department == null)
            return false;

        _dbContext.Departments.Remove(department);
        int rows = await _dbContext.SaveChangesAsync(cancellationToken);
        return rows > 0;
    }

    public async Task<IReadOnlyList<SemesterDetailsDto>> GetAllSemestersAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<SemesterDetailsDto>();
        const string sql = "SELECT SemesterId, SemesterName, AcademicYear, StartDate, EndDate, IsCurrent FROM Semesters ORDER BY SemesterId DESC;";
        await using var cmd = await CreateCommandAsync(sql);
        await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await r.ReadAsync(cancellationToken))
        {
            list.Add(new SemesterDetailsDto(
                r.GetInt32("SemesterId"),
                r.GetString("SemesterName"),
                r["AcademicYear"].ToString() ?? "",
                DateOnly.FromDateTime(r.GetDateTime("StartDate")),
                DateOnly.FromDateTime(r.GetDateTime("EndDate")),
                r.GetBoolean("IsCurrent")
            ));
        }
        return list;
    }

    public async Task<SemesterDetailsDto?> GetSemesterByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT SemesterId, SemesterName, AcademicYear, StartDate, EndDate, IsCurrent FROM Semesters WHERE SemesterId = @id;";
        await using var cmd = await CreateCommandAsync(sql);
        cmd.Parameters.AddWithValue("@id", id);
        await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await r.ReadAsync(cancellationToken))
        {
            return new SemesterDetailsDto(
                r.GetInt32("SemesterId"),
                r.GetString("SemesterName"),
                r["AcademicYear"].ToString() ?? "",
                DateOnly.FromDateTime(r.GetDateTime("StartDate")),
                DateOnly.FromDateTime(r.GetDateTime("EndDate")),
                r.GetBoolean("IsCurrent")
            );
        }
        return null;
    }

    public async Task<int> CreateSemesterAsync(CreateUpdateSemesterDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.IsCurrent)
        {
            const string clearCurrentSql = "UPDATE Semesters SET IsCurrent = 0;";
            await using var clearCmd = await CreateCommandAsync(clearCurrentSql);
            await clearCmd.ExecuteNonQueryAsync(cancellationToken);
        }

        const string sql = "INSERT INTO Semesters (SemesterName, AcademicYear, StartDate, EndDate, IsCurrent) VALUES (@name, @year, @start, @end, @isCurrent); SELECT LAST_INSERT_ID();";
        await using var cmd = await CreateCommandAsync(sql);
        cmd.Parameters.AddWithValue("@name", dto.SemesterName);
        cmd.Parameters.AddWithValue("@year", int.TryParse(dto.AcademicYear, out int yr) ? yr : dto.AcademicYear);
        cmd.Parameters.AddWithValue("@start", dto.StartDate.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@end", dto.EndDate.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@isCurrent", dto.IsCurrent);
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdateSemesterAsync(int id, CreateUpdateSemesterDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.IsCurrent)
        {
            const string clearCurrentSql = "UPDATE Semesters SET IsCurrent = 0 WHERE SemesterId <> @id;";
            await using var clearCmd = await CreateCommandAsync(clearCurrentSql);
            clearCmd.Parameters.AddWithValue("@id", id);
            await clearCmd.ExecuteNonQueryAsync(cancellationToken);
        }

        const string sql = "UPDATE Semesters SET SemesterName = @name, AcademicYear = @year, StartDate = @start, EndDate = @end, IsCurrent = @isCurrent WHERE SemesterId = @id;";
        await using var cmd = await CreateCommandAsync(sql);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@name", dto.SemesterName);
        cmd.Parameters.AddWithValue("@year", int.TryParse(dto.AcademicYear, out int yr) ? yr : dto.AcademicYear);
        cmd.Parameters.AddWithValue("@start", dto.StartDate.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@end", dto.EndDate.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@isCurrent", dto.IsCurrent);
        int rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    public async Task<bool> DeleteSemesterAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM Semesters WHERE SemesterId = @id;";
        await using var cmd = await CreateCommandAsync(sql);
        cmd.Parameters.AddWithValue("@id", id);
        int rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    public async Task<IReadOnlyList<RoomDetailsDto>> GetAllRoomsAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<RoomDetailsDto>();
        const string sql = "SELECT RoomId, BuildingName, RoomNumber, Capacity FROM Rooms ORDER BY BuildingName, RoomNumber ASC;";
        await using var cmd = await CreateCommandAsync(sql);
        await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await r.ReadAsync(cancellationToken))
        {
            list.Add(new RoomDetailsDto(
                r.GetInt32("RoomId"),
                r.GetString("BuildingName"),
                r.GetString("RoomNumber"),
                r.GetInt32("Capacity")
            ));
        }
        return list;
    }

    public async Task<RoomDetailsDto?> GetRoomByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT RoomId, BuildingName, RoomNumber, Capacity FROM Rooms WHERE RoomId = @id;";
        await using var cmd = await CreateCommandAsync(sql);
        cmd.Parameters.AddWithValue("@id", id);
        await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await r.ReadAsync(cancellationToken))
        {
            return new RoomDetailsDto(
                r.GetInt32("RoomId"),
                r.GetString("BuildingName"),
                r.GetString("RoomNumber"),
                r.GetInt32("Capacity")
            );
        }
        return null;
    }

    public async Task<int> CreateRoomAsync(CreateUpdateRoomDto dto, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT INTO Rooms (BuildingName, RoomNumber, Capacity) VALUES (@bldg, @room, @cap); SELECT LAST_INSERT_ID();";
        await using var cmd = await CreateCommandAsync(sql);
        cmd.Parameters.AddWithValue("@bldg", dto.BuildingName);
        cmd.Parameters.AddWithValue("@room", dto.RoomNumber);
        cmd.Parameters.AddWithValue("@cap", dto.Capacity);
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdateRoomAsync(int id, CreateUpdateRoomDto dto, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE Rooms SET BuildingName = @bldg, RoomNumber = @room, Capacity = @cap WHERE RoomId = @id;";
        await using var cmd = await CreateCommandAsync(sql);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@bldg", dto.BuildingName);
        cmd.Parameters.AddWithValue("@room", dto.RoomNumber);
        cmd.Parameters.AddWithValue("@cap", dto.Capacity);
        int rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    public async Task<bool> DeleteRoomAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM Rooms WHERE RoomId = @id;";
        await using var cmd = await CreateCommandAsync(sql);
        cmd.Parameters.AddWithValue("@id", id);
        int rows = await cmd.ExecuteNonQueryAsync(cancellationToken);

        return rows > 0;
    }

    private static readonly Dictionary<string, (string PrimaryKey, bool HasDescription)> StatusTables = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CourseStatuses"] = ("CourseStatusId", true),
        ["StudentStatuses"] = ("StudentStatusId", true),
        ["InstructorStatuses"] = ("InstructorStatusId", true),
        ["OfferingStatuses"] = ("OfferingStatusId", true),
        ["EnrollmentStatuses"] = ("EnrollmentStatusId", true),
        ["AttendanceStatuses"] = ("AttendanceStatusId", false)
    };

    private static (string TableName, string PrimaryKey, bool HasDescription) ValidateStatusTable(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName) || !StatusTables.TryGetValue(tableName, out var info))
        {
            throw new ArgumentException($"Invalid lookup table name '{tableName}'. Allowed: {string.Join(", ", StatusTables.Keys)}", nameof(tableName));
        }
        return (StatusTables.Keys.First(k => k.Equals(tableName, StringComparison.OrdinalIgnoreCase)), info.PrimaryKey, info.HasDescription);
    }

    public async Task<IReadOnlyList<StatusLookupItemDto>> GetStatusItemsAsync(string tableName, CancellationToken cancellationToken = default)
    {
        var (table, pk, hasDesc) = ValidateStatusTable(tableName);
        var list = new List<StatusLookupItemDto>();
        string sql = hasDesc
            ? $"SELECT {pk} AS Id, StatusName, Description FROM {table} ORDER BY {pk} ASC;"
            : $"SELECT {pk} AS Id, StatusName, NULL AS Description FROM {table} ORDER BY {pk} ASC;";

        await using var cmd = await CreateCommandAsync(sql);
        await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await r.ReadAsync(cancellationToken))
        {
            list.Add(new StatusLookupItemDto(
                r.GetInt32("Id"),
                r.GetString("StatusName"),
                r.IsDBNull(r.GetOrdinal("Description")) ? null : r.GetString("Description")
            ));
        }
        return list;
    }

    public async Task<StatusLookupItemDto?> GetStatusItemByIdAsync(string tableName, int id, CancellationToken cancellationToken = default)
    {
        var (table, pk, hasDesc) = ValidateStatusTable(tableName);
        string sql = hasDesc
            ? $"SELECT {pk} AS Id, StatusName, Description FROM {table} WHERE {pk} = @id;"
            : $"SELECT {pk} AS Id, StatusName, NULL AS Description FROM {table} WHERE {pk} = @id;";

        await using var cmd = await CreateCommandAsync(sql);
        cmd.Parameters.AddWithValue("@id", id);
        await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await r.ReadAsync(cancellationToken))
        {
            return new StatusLookupItemDto(
                r.GetInt32("Id"),
                r.GetString("StatusName"),
                r.IsDBNull(r.GetOrdinal("Description")) ? null : r.GetString("Description")
            );
        }
        return null;
    }

    public async Task<int> CreateStatusItemAsync(string tableName, CreateUpdateStatusItemDto dto, CancellationToken cancellationToken = default)
    {
        var (table, pk, hasDesc) = ValidateStatusTable(tableName);
        string sql = hasDesc
            ? $"INSERT INTO {table} (StatusName, Description) VALUES (@name, @desc); SELECT LAST_INSERT_ID();"
            : $"INSERT INTO {table} (StatusName) VALUES (@name); SELECT LAST_INSERT_ID();";

        await using var cmd = await CreateCommandAsync(sql);
        cmd.Parameters.AddWithValue("@name", dto.StatusName);
        if (hasDesc)
        {
            cmd.Parameters.AddWithValue("@desc", (object?)dto.Description ?? DBNull.Value);
        }
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdateStatusItemAsync(string tableName, int id, CreateUpdateStatusItemDto dto, CancellationToken cancellationToken = default)
    {
        var (table, pk, hasDesc) = ValidateStatusTable(tableName);
        string sql = hasDesc
            ? $"UPDATE {table} SET StatusName = @name, Description = @desc WHERE {pk} = @id;"
            : $"UPDATE {table} SET StatusName = @name WHERE {pk} = @id;";

        await using var cmd = await CreateCommandAsync(sql);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@name", dto.StatusName);
        if (hasDesc)
        {
            cmd.Parameters.AddWithValue("@desc", (object?)dto.Description ?? DBNull.Value);
        }
        int rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    public async Task<bool> DeleteStatusItemAsync(string tableName, int id, CancellationToken cancellationToken = default)
    {
        var (table, pk, _) = ValidateStatusTable(tableName);
        string sql = $"DELETE FROM {table} WHERE {pk} = @id;";
        await using var cmd = await CreateCommandAsync(sql);
        cmd.Parameters.AddWithValue("@id", id);
        int rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }
}

