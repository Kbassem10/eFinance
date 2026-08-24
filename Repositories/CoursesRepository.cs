using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using StudentRegistrationPortal.Api.DTOs;

namespace StudentRegistrationPortal.Api.Repositories;

/// Courses Repository executing standard SQL queries using the shared Unit of Work connection and transaction.
public class CoursesRepository : ICoursesRepository
{
    private readonly Func<Task<MySqlConnection>> _connectionProvider;
    private readonly Func<MySqlTransaction?> _transactionProvider;
    private readonly ILogger<CoursesRepository> _logger;

    public CoursesRepository(
        Func<Task<MySqlConnection>> connectionProvider,
        Func<MySqlTransaction?> transactionProvider,
        ILogger<CoursesRepository> logger)
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

    public async Task<IReadOnlyList<CourseDetailsDto>> GetAllAsync(int? departmentId = null, int? statusId = null, CancellationToken cancellationToken = default)
    {
        var list = new List<CourseDetailsDto>();

        try
        {
            var sql = @"
                SELECT 
                    c.CourseId,
                    c.CourseCode,
                    c.CourseName,
                    c.CreditHours,
                    c.DifficultyLevel,
                    c.CourseStatusId,
                    cs.StatusName,
                    GROUP_CONCAT(DISTINCT d.DepartmentName SEPARATOR ', ') AS AssignedDepartments,
                    c.CreatedAt,
                    c.UpdatedAt
                FROM Courses c
                INNER JOIN CourseStatuses cs ON c.CourseStatusId = cs.CourseStatusId
                LEFT JOIN CourseDepartments cd ON c.CourseId = cd.CourseId
                LEFT JOIN Departments d ON cd.DepartmentId = d.DepartmentId
                WHERE (@DepartmentId IS NULL OR cd.DepartmentId = @DepartmentId)
                  AND (@StatusId IS NULL OR c.CourseStatusId = @StatusId)
                GROUP BY c.CourseId, c.CourseCode, c.CourseName, c.CreditHours, c.DifficultyLevel, c.CourseStatusId, cs.StatusName, c.CreatedAt, c.UpdatedAt
                ORDER BY c.CourseId ASC;";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@DepartmentId", (object?)departmentId ?? DBNull.Value);
            command.Parameters.AddWithValue("@StatusId", (object?)statusId ?? DBNull.Value);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                list.Add(MapCourseSummaryFromReader(reader));
            }

            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving courses list");
            throw;
        }
    }

    public async Task<CourseDetailsDto?> GetByIdAsync(int courseId, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = @"
                SELECT 
                    c.CourseId,
                    c.CourseCode,
                    c.CourseName,
                    c.CreditHours,
                    c.DifficultyLevel,
                    c.CourseStatusId,
                    cs.StatusName,
                    GROUP_CONCAT(DISTINCT d.DepartmentName SEPARATOR ', ') AS AssignedDepartments,
                    c.CreatedAt,
                    c.UpdatedAt
                FROM Courses c
                INNER JOIN CourseStatuses cs ON c.CourseStatusId = cs.CourseStatusId
                LEFT JOIN CourseDepartments cd ON c.CourseId = cd.CourseId
                LEFT JOIN Departments d ON cd.DepartmentId = d.DepartmentId
                WHERE c.CourseId = @CourseId
                GROUP BY c.CourseId, c.CourseCode, c.CourseName, c.CreditHours, c.DifficultyLevel, c.CourseStatusId, cs.StatusName, c.CreatedAt, c.UpdatedAt
                LIMIT 1;";

            CourseDetailsDto? course = null;
            await using (var command = await CreateCommandAsync(sql))
            {
                command.Parameters.AddWithValue("@CourseId", courseId);
                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                if (await reader.ReadAsync(cancellationToken))
                {
                    course = MapCourseSummaryFromReader(reader);
                }
            }

            if (course == null) return null;

            var deptIds = await GetDepartmentIdsForCourseAsync(courseId, cancellationToken);
            var (prereqIds, prereqCodes) = await GetPrerequisitesMetaForCourseAsync(courseId, cancellationToken);

            return course with
            {
                DepartmentIds = deptIds,
                PrerequisiteCourseIds = prereqIds,
                PrerequisiteCourseCodes = prereqCodes
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course ID {CourseId}", courseId);
            throw;
        }
    }

    public async Task<CourseDetailsDto?> GetByCourseCodeAsync(string courseCode, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = @"
                SELECT 
                    c.CourseId,
                    c.CourseCode,
                    c.CourseName,
                    c.CreditHours,
                    c.DifficultyLevel,
                    c.CourseStatusId,
                    cs.StatusName,
                    GROUP_CONCAT(DISTINCT d.DepartmentName SEPARATOR ', ') AS AssignedDepartments,
                    c.CreatedAt,
                    c.UpdatedAt
                FROM Courses c
                INNER JOIN CourseStatuses cs ON c.CourseStatusId = cs.CourseStatusId
                LEFT JOIN CourseDepartments cd ON c.CourseId = cd.CourseId
                LEFT JOIN Departments d ON cd.DepartmentId = d.DepartmentId
                WHERE c.CourseCode = @CourseCode
                GROUP BY c.CourseId, c.CourseCode, c.CourseName, c.CreditHours, c.DifficultyLevel, c.CourseStatusId, cs.StatusName, c.CreatedAt, c.UpdatedAt
                LIMIT 1;";

            CourseDetailsDto? course = null;
            await using (var command = await CreateCommandAsync(sql))
            {
                command.Parameters.AddWithValue("@CourseCode", courseCode);
                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                if (await reader.ReadAsync(cancellationToken))
                {
                    course = MapCourseSummaryFromReader(reader);
                }
            }

            if (course == null) return null;

            var deptIds = await GetDepartmentIdsForCourseAsync(course.CourseId, cancellationToken);
            var (prereqIds, prereqCodes) = await GetPrerequisitesMetaForCourseAsync(course.CourseId, cancellationToken);

            return course with
            {
                DepartmentIds = deptIds,
                PrerequisiteCourseIds = prereqIds,
                PrerequisiteCourseCodes = prereqCodes
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course by code '{CourseCode}'", courseCode);
            throw;
        }
    }

    public async Task<int> CreateAsync(CreateCourseDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = @"
                INSERT INTO Courses (
                    CourseCode, CourseName, CreditHours, DifficultyLevel, CourseStatusId, CreatedAt, UpdatedAt
                ) VALUES (
                    @CourseCode, @CourseName, @CreditHours, @DifficultyLevel, @CourseStatusId, UTC_TIMESTAMP(), UTC_TIMESTAMP()
                );
                SELECT LAST_INSERT_ID();";

            int courseId;
            await using (var command = await CreateCommandAsync(sql))
            {
                command.Parameters.AddWithValue("@CourseCode", dto.CourseCode.Trim());
                command.Parameters.AddWithValue("@CourseName", dto.CourseName.Trim());
                command.Parameters.AddWithValue("@CreditHours", dto.CreditHours);
                command.Parameters.AddWithValue("@DifficultyLevel", dto.DifficultyLevel);
                command.Parameters.AddWithValue("@CourseStatusId", dto.CourseStatusId);

                var insertedId = await command.ExecuteScalarAsync(cancellationToken);
                courseId = Convert.ToInt32(insertedId);
            }

            if (dto.DepartmentIds is { Count: > 0 })
            {
                foreach (var deptId in dto.DepartmentIds.Distinct())
                {
                    await using var deptCommand = await CreateCommandAsync(
                        "INSERT IGNORE INTO CourseDepartments (CourseId, DepartmentId) VALUES (@CourseId, @DepartmentId);");
                    deptCommand.Parameters.AddWithValue("@CourseId", courseId);
                    deptCommand.Parameters.AddWithValue("@DepartmentId", deptId);
                    await deptCommand.ExecuteNonQueryAsync(cancellationToken);
                }
            }

            if (dto.PrerequisiteCourseIds is { Count: > 0 })
            {
                foreach (var prereqId in dto.PrerequisiteCourseIds.Distinct())
                {
                    if (prereqId == courseId) continue;
                    await using var prereqCommand = await CreateCommandAsync(
                        "INSERT IGNORE INTO CoursePrerequisites (CourseId, PrerequisiteCourseId) VALUES (@CourseId, @PrerequisiteId);");
                    prereqCommand.Parameters.AddWithValue("@CourseId", courseId);
                    prereqCommand.Parameters.AddWithValue("@PrerequisiteId", prereqId);
                    await prereqCommand.ExecuteNonQueryAsync(cancellationToken);
                }
            }

            return courseId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course code '{CourseCode}'", dto.CourseCode);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int courseId, UpdateCourseDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = @"
                UPDATE Courses
                SET CourseName = @CourseName,
                    CreditHours = @CreditHours,
                    DifficultyLevel = @DifficultyLevel,
                    CourseStatusId = @CourseStatusId,
                    UpdatedAt = UTC_TIMESTAMP()
                WHERE CourseId = @CourseId;";

            int rowsAffected;
            await using (var command = await CreateCommandAsync(sql))
            {
                command.Parameters.AddWithValue("@CourseId", courseId);
                command.Parameters.AddWithValue("@CourseName", dto.CourseName.Trim());
                command.Parameters.AddWithValue("@CreditHours", dto.CreditHours);
                command.Parameters.AddWithValue("@DifficultyLevel", dto.DifficultyLevel);
                command.Parameters.AddWithValue("@CourseStatusId", dto.CourseStatusId);

                rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
            }

            if (rowsAffected == 0) return false;

            if (dto.DepartmentIds != null)
            {
                await using (var deleteDepts = await CreateCommandAsync("DELETE FROM CourseDepartments WHERE CourseId = @CourseId;"))
                {
                    deleteDepts.Parameters.AddWithValue("@CourseId", courseId);
                    await deleteDepts.ExecuteNonQueryAsync(cancellationToken);
                }

                foreach (var deptId in dto.DepartmentIds.Distinct())
                {
                    await using var insertDept = await CreateCommandAsync(
                        "INSERT INTO CourseDepartments (CourseId, DepartmentId) VALUES (@CourseId, @DepartmentId);");
                    insertDept.Parameters.AddWithValue("@CourseId", courseId);
                    insertDept.Parameters.AddWithValue("@DepartmentId", deptId);
                    await insertDept.ExecuteNonQueryAsync(cancellationToken);
                }
            }

            if (dto.PrerequisiteCourseIds != null)
            {
                await using (var deletePrereqs = await CreateCommandAsync("DELETE FROM CoursePrerequisites WHERE CourseId = @CourseId;"))
                {
                    deletePrereqs.Parameters.AddWithValue("@CourseId", courseId);
                    await deletePrereqs.ExecuteNonQueryAsync(cancellationToken);
                }

                foreach (var prereqId in dto.PrerequisiteCourseIds.Distinct())
                {
                    if (prereqId == courseId) continue;
                    await using var insertPrereq = await CreateCommandAsync(
                        "INSERT INTO CoursePrerequisites (CourseId, PrerequisiteCourseId) VALUES (@CourseId, @PrerequisiteId);");
                    insertPrereq.Parameters.AddWithValue("@CourseId", courseId);
                    insertPrereq.Parameters.AddWithValue("@PrerequisiteId", prereqId);
                    await insertPrereq.ExecuteNonQueryAsync(cancellationToken);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating course ID {CourseId}", courseId);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int courseId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Remove prerequisites junctions first
            await using (var delPrereqs = await CreateCommandAsync(
                "DELETE FROM CoursePrerequisites WHERE CourseId = @CourseId OR PrerequisiteCourseId = @CourseId;"))
            {
                delPrereqs.Parameters.AddWithValue("@CourseId", courseId);
                await delPrereqs.ExecuteNonQueryAsync(cancellationToken);
            }

            const string sql = "DELETE FROM Courses WHERE CourseId = @CourseId;";
            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@CourseId", courseId);

            int rows = await command.ExecuteNonQueryAsync(cancellationToken);
            return rows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting course ID {CourseId}", courseId);
            throw;
        }
    }

    public async Task<IReadOnlyList<CourseDetailsDto>> GetPrerequisitesAsync(int courseId, CancellationToken cancellationToken = default)
    {
        var list = new List<CourseDetailsDto>();

        try
        {
            const string sql = @"
                SELECT 
                    c.CourseId,
                    c.CourseCode,
                    c.CourseName,
                    c.CreditHours,
                    c.DifficultyLevel,
                    c.CourseStatusId,
                    cs.StatusName,
                    GROUP_CONCAT(DISTINCT d.DepartmentName SEPARATOR ', ') AS AssignedDepartments,
                    c.CreatedAt,
                    c.UpdatedAt
                FROM CoursePrerequisites cp
                INNER JOIN Courses c ON cp.PrerequisiteCourseId = c.CourseId
                INNER JOIN CourseStatuses cs ON c.CourseStatusId = cs.CourseStatusId
                LEFT JOIN CourseDepartments cd ON c.CourseId = cd.CourseId
                LEFT JOIN Departments d ON cd.DepartmentId = d.DepartmentId
                WHERE cp.CourseId = @CourseId
                GROUP BY c.CourseId, c.CourseCode, c.CourseName, c.CreditHours, c.DifficultyLevel, c.CourseStatusId, cs.StatusName, c.CreatedAt, c.UpdatedAt
                ORDER BY c.CourseCode ASC;";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@CourseId", courseId);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                list.Add(MapCourseSummaryFromReader(reader));
            }

            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving prerequisites for course ID {CourseId}", courseId);
            throw;
        }
    }

    private async Task<IReadOnlyList<int>> GetDepartmentIdsForCourseAsync(int courseId, CancellationToken cancellationToken)
    {
        var ids = new List<int>();
        const string sql = "SELECT DepartmentId FROM CourseDepartments WHERE CourseId = @CourseId;";

        await using var command = await CreateCommandAsync(sql);
        command.Parameters.AddWithValue("@CourseId", courseId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            ids.Add(reader.GetInt32("DepartmentId"));
        }

        return ids;
    }

    private async Task<(IReadOnlyList<int> Ids, IReadOnlyList<string> Codes)> GetPrerequisitesMetaForCourseAsync(int courseId, CancellationToken cancellationToken)
    {
        var ids = new List<int>();
        var codes = new List<string>();

        const string sql = @"
            SELECT cp.PrerequisiteCourseId, c.CourseCode
            FROM CoursePrerequisites cp
            INNER JOIN Courses c ON cp.PrerequisiteCourseId = c.CourseId
            WHERE cp.CourseId = @CourseId;";

        await using var command = await CreateCommandAsync(sql);
        command.Parameters.AddWithValue("@CourseId", courseId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            ids.Add(reader.GetInt32("PrerequisiteCourseId"));
            codes.Add(reader.GetString("CourseCode"));
        }

        return (ids, codes);
    }

    private static CourseDetailsDto MapCourseSummaryFromReader(DbDataReader reader)
    {
        return new CourseDetailsDto
        {
            CourseId = reader.GetInt32("CourseId"),
            CourseCode = reader.GetString("CourseCode"),
            CourseName = reader.GetString("CourseName"),
            CreditHours = reader.GetInt32("CreditHours"),
            DifficultyLevel = reader.GetString("DifficultyLevel"),
            CourseStatusId = reader.GetInt32("CourseStatusId"),
            StatusName = reader.GetString("StatusName"),
            AssignedDepartments = reader.IsDBNull("AssignedDepartments") ? null : reader.GetString("AssignedDepartments"),
            CreatedAt = reader.GetDateTime("CreatedAt"),
            UpdatedAt = reader.GetDateTime("UpdatedAt")
        };
    }
}

