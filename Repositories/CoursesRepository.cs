using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using StudentRegistrationPortal.Api.DTOs;

namespace StudentRegistrationPortal.Api.Repositories;

/// <summary>
/// Courses Repository executing relational CRUD queries for Course, CourseDepartment,
/// CourseOffering, CourseOfferingInstructor, CourseSchedule, CoursePrerequisite, and CourseStatus.
/// </summary>
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
                    GROUP_CONCAT(DISTINCT CONCAT(i.FirstName, ' ', i.LastName) SEPARATOR ', ') AS InstructorName,
                    c.CreatedAt,
                    c.UpdatedAt
                FROM Courses c
                INNER JOIN CourseStatuses cs ON c.CourseStatusId = cs.CourseStatusId
                LEFT JOIN CourseDepartments cd ON c.CourseId = cd.CourseId
                LEFT JOIN Departments d ON cd.DepartmentId = d.DepartmentId
                LEFT JOIN CourseOfferings co ON c.CourseId = co.CourseId
                LEFT JOIN CourseOfferingInstructors coi ON co.CourseOfferingId = coi.CourseOfferingId
                LEFT JOIN Instructors i ON coi.InstructorId = i.InstructorId
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
                    GROUP_CONCAT(DISTINCT CONCAT(i.FirstName, ' ', i.LastName) SEPARATOR ', ') AS InstructorName,
                    c.CreatedAt,
                    c.UpdatedAt
                FROM Courses c
                INNER JOIN CourseStatuses cs ON c.CourseStatusId = cs.CourseStatusId
                LEFT JOIN CourseDepartments cd ON c.CourseId = cd.CourseId
                LEFT JOIN Departments d ON cd.DepartmentId = d.DepartmentId
                LEFT JOIN CourseOfferings co ON c.CourseId = co.CourseId
                LEFT JOIN CourseOfferingInstructors coi ON co.CourseOfferingId = coi.CourseOfferingId
                LEFT JOIN Instructors i ON coi.InstructorId = i.InstructorId
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
            var offerings = await GetOfferingsForCourseAsync(courseId, cancellationToken);

            return course with
            {
                DepartmentIds = deptIds,
                PrerequisiteCourseIds = prereqIds,
                PrerequisiteCourseCodes = prereqCodes,
                Offerings = offerings
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
                    GROUP_CONCAT(DISTINCT CONCAT(i.FirstName, ' ', i.LastName) SEPARATOR ', ') AS InstructorName,
                    c.CreatedAt,
                    c.UpdatedAt
                FROM Courses c
                INNER JOIN CourseStatuses cs ON c.CourseStatusId = cs.CourseStatusId
                LEFT JOIN CourseDepartments cd ON c.CourseId = cd.CourseId
                LEFT JOIN Departments d ON cd.DepartmentId = d.DepartmentId
                LEFT JOIN CourseOfferings co ON c.CourseId = co.CourseId
                LEFT JOIN CourseOfferingInstructors coi ON co.CourseOfferingId = coi.CourseOfferingId
                LEFT JOIN Instructors i ON coi.InstructorId = i.InstructorId
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
            var offerings = await GetOfferingsForCourseAsync(course.CourseId, cancellationToken);

            return course with
            {
                DepartmentIds = deptIds,
                PrerequisiteCourseIds = prereqIds,
                PrerequisiteCourseCodes = prereqCodes,
                Offerings = offerings
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

            // 1. Insert CourseDepartments
            if (dto.DepartmentIds is { Count: > 0 })
            {
                foreach (var deptId in dto.DepartmentIds.Distinct())
                {
                    int validDeptId = await EnsureValidDepartmentIdAsync(deptId, cancellationToken);
                    await using var deptCommand = await CreateCommandAsync(
                        "INSERT IGNORE INTO CourseDepartments (CourseId, DepartmentId) VALUES (@CourseId, @DepartmentId);");
                    deptCommand.Parameters.AddWithValue("@CourseId", courseId);
                    deptCommand.Parameters.AddWithValue("@DepartmentId", validDeptId);
                    await deptCommand.ExecuteNonQueryAsync(cancellationToken);
                }
            }

            // 2. Insert CoursePrerequisites
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

            // 3. Insert CourseOfferings, Instructors, and Schedules
            if (dto.Offerings is { Count: > 0 })
            {
                foreach (var off in dto.Offerings)
                {
                    int validSemesterId = await EnsureValidSemesterIdAsync(off.SemesterId, cancellationToken);

                    const string offSql = @"
                        INSERT INTO CourseOfferings (CourseId, SemesterId, OfferingStatusId, SectionNumber, Capacity, CreatedAt)
                        VALUES (@CourseId, @SemesterId, @OfferingStatusId, @SectionNumber, @Capacity, UTC_TIMESTAMP());
                        SELECT LAST_INSERT_ID();";

                    int offeringId;
                    await using (var offCmd = await CreateCommandAsync(offSql))
                    {
                        offCmd.Parameters.AddWithValue("@CourseId", courseId);
                        offCmd.Parameters.AddWithValue("@SemesterId", validSemesterId);
                        offCmd.Parameters.AddWithValue("@OfferingStatusId", off.OfferingStatusId <= 0 ? 1 : off.OfferingStatusId);
                        offCmd.Parameters.AddWithValue("@SectionNumber", string.IsNullOrWhiteSpace(off.SectionNumber) ? "SEC-01" : off.SectionNumber);
                        offCmd.Parameters.AddWithValue("@Capacity", off.Capacity <= 0 ? 30 : off.Capacity);

                        var res = await offCmd.ExecuteScalarAsync(cancellationToken);
                        offeringId = Convert.ToInt32(res);
                    }

                    if (off.InstructorIds is { Count: > 0 })
                    {
                        bool isFirst = true;
                        foreach (var instId in off.InstructorIds.Distinct())
                        {
                            int? validInstId = await ValidateInstructorIdAsync(instId, cancellationToken);
                            if (validInstId.HasValue)
                            {
                                await using var instCmd = await CreateCommandAsync(
                                    "INSERT IGNORE INTO CourseOfferingInstructors (CourseOfferingId, InstructorId, IsPrimary) VALUES (@OfferingId, @InstructorId, @IsPrimary);");
                                instCmd.Parameters.AddWithValue("@OfferingId", offeringId);
                                instCmd.Parameters.AddWithValue("@InstructorId", validInstId.Value);
                                instCmd.Parameters.AddWithValue("@IsPrimary", isFirst ? 1 : 0);
                                await instCmd.ExecuteNonQueryAsync(cancellationToken);
                                isFirst = false;
                            }
                        }
                    }

                    if (off.Schedules is { Count: > 0 })
                    {
                        foreach (var sch in off.Schedules)
                        {
                            int validRoomId = await EnsureValidRoomIdAsync(sch.RoomId, cancellationToken);
                            await using var schCmd = await CreateCommandAsync(
                                "INSERT INTO CourseSchedules (CourseOfferingId, RoomId, DayOfWeek, StartTime, EndTime) VALUES (@OfferingId, @RoomId, @DayOfWeek, @StartTime, @EndTime);");
                            schCmd.Parameters.AddWithValue("@OfferingId", offeringId);
                            schCmd.Parameters.AddWithValue("@RoomId", validRoomId);
                            schCmd.Parameters.AddWithValue("@DayOfWeek", sch.DayOfWeek <= 0 ? 1 : sch.DayOfWeek);
                            schCmd.Parameters.AddWithValue("@StartTime", string.IsNullOrWhiteSpace(sch.StartTime) ? "09:00:00" : sch.StartTime);
                            schCmd.Parameters.AddWithValue("@EndTime", string.IsNullOrWhiteSpace(sch.EndTime) ? "10:30:00" : sch.EndTime);
                            await schCmd.ExecuteNonQueryAsync(cancellationToken);
                        }
                    }
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

            // 1. Sync Departments
            if (dto.DepartmentIds != null)
            {
                await using (var deleteDepts = await CreateCommandAsync("DELETE FROM CourseDepartments WHERE CourseId = @CourseId;"))
                {
                    deleteDepts.Parameters.AddWithValue("@CourseId", courseId);
                    await deleteDepts.ExecuteNonQueryAsync(cancellationToken);
                }

                foreach (var deptId in dto.DepartmentIds.Distinct())
                {
                    int validDeptId = await EnsureValidDepartmentIdAsync(deptId, cancellationToken);
                    await using var insertDept = await CreateCommandAsync(
                        "INSERT INTO CourseDepartments (CourseId, DepartmentId) VALUES (@CourseId, @DepartmentId);");
                    insertDept.Parameters.AddWithValue("@CourseId", courseId);
                    insertDept.Parameters.AddWithValue("@DepartmentId", validDeptId);
                    await insertDept.ExecuteNonQueryAsync(cancellationToken);
                }
            }

            // 2. Sync Prerequisites
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

            // 3. Sync Offerings if provided
            if (dto.Offerings is { Count: > 0 })
            {
                foreach (var off in dto.Offerings)
                {
                    int validSemesterId = await EnsureValidSemesterIdAsync(off.SemesterId, cancellationToken);

                    int offeringId = 0;
                    const string findSql = "SELECT CourseOfferingId FROM CourseOfferings WHERE CourseId = @CourseId AND SemesterId = @SemesterId LIMIT 1;";
                    await using (var findCmd = await CreateCommandAsync(findSql))
                    {
                        findCmd.Parameters.AddWithValue("@CourseId", courseId);
                        findCmd.Parameters.AddWithValue("@SemesterId", validSemesterId);
                        var sc = await findCmd.ExecuteScalarAsync(cancellationToken);
                        if (sc != null && sc != DBNull.Value) offeringId = Convert.ToInt32(sc);
                    }

                    if (offeringId == 0)
                    {
                        const string createOffSql = @"
                            INSERT INTO CourseOfferings (CourseId, SemesterId, OfferingStatusId, SectionNumber, Capacity, CreatedAt)
                            VALUES (@CourseId, @SemesterId, @OfferingStatusId, @SectionNumber, @Capacity, UTC_TIMESTAMP());
                            SELECT LAST_INSERT_ID();";

                        await using var createCmd = await CreateCommandAsync(createOffSql);
                        createCmd.Parameters.AddWithValue("@CourseId", courseId);
                        createCmd.Parameters.AddWithValue("@SemesterId", validSemesterId);
                        createCmd.Parameters.AddWithValue("@OfferingStatusId", off.OfferingStatusId <= 0 ? 1 : off.OfferingStatusId);
                        createCmd.Parameters.AddWithValue("@SectionNumber", off.SectionNumber);
                        createCmd.Parameters.AddWithValue("@Capacity", off.Capacity <= 0 ? 30 : off.Capacity);
                        var res = await createCmd.ExecuteScalarAsync(cancellationToken);
                        offeringId = Convert.ToInt32(res);
                    }
                    else
                    {
                        const string updOffSql = @"
                            UPDATE CourseOfferings 
                            SET OfferingStatusId = @OfferingStatusId, SectionNumber = @SectionNumber, Capacity = @Capacity 
                            WHERE CourseOfferingId = @OfferingId;";

                        await using var updCmd = await CreateCommandAsync(updOffSql);
                        updCmd.Parameters.AddWithValue("@OfferingStatusId", off.OfferingStatusId <= 0 ? 1 : off.OfferingStatusId);
                        updCmd.Parameters.AddWithValue("@SectionNumber", off.SectionNumber);
                        updCmd.Parameters.AddWithValue("@Capacity", off.Capacity <= 0 ? 30 : off.Capacity);
                        updCmd.Parameters.AddWithValue("@OfferingId", offeringId);
                        await updCmd.ExecuteNonQueryAsync(cancellationToken);
                    }

                    // Sync instructors for this offering
                    if (off.InstructorIds != null)
                    {
                        await using var delInst = await CreateCommandAsync("DELETE FROM CourseOfferingInstructors WHERE CourseOfferingId = @OfferingId;");
                        delInst.Parameters.AddWithValue("@OfferingId", offeringId);
                        await delInst.ExecuteNonQueryAsync(cancellationToken);

                        bool isFirst = true;
                        foreach (var instId in off.InstructorIds.Distinct())
                        {
                            int? validInstId = await ValidateInstructorIdAsync(instId, cancellationToken);
                            if (validInstId.HasValue)
                            {
                                await using var insInst = await CreateCommandAsync(
                                    "INSERT IGNORE INTO CourseOfferingInstructors (CourseOfferingId, InstructorId, IsPrimary) VALUES (@OfferingId, @InstructorId, @IsPrimary);");
                                insInst.Parameters.AddWithValue("@OfferingId", offeringId);
                                insInst.Parameters.AddWithValue("@InstructorId", validInstId.Value);
                                insInst.Parameters.AddWithValue("@IsPrimary", isFirst ? 1 : 0);
                                await insInst.ExecuteNonQueryAsync(cancellationToken);
                                isFirst = false;
                            }
                        }
                    }

                    // Sync schedules for this offering
                    if (off.Schedules != null)
                    {
                        await using var delSch = await CreateCommandAsync("DELETE FROM CourseSchedules WHERE CourseOfferingId = @OfferingId;");
                        delSch.Parameters.AddWithValue("@OfferingId", offeringId);
                        await delSch.ExecuteNonQueryAsync(cancellationToken);

                        foreach (var sch in off.Schedules)
                        {
                            int validRoomId = await EnsureValidRoomIdAsync(sch.RoomId, cancellationToken);
                            await using var insSch = await CreateCommandAsync(
                                "INSERT INTO CourseSchedules (CourseOfferingId, RoomId, DayOfWeek, StartTime, EndTime) VALUES (@OfferingId, @RoomId, @DayOfWeek, @StartTime, @EndTime);");
                            insSch.Parameters.AddWithValue("@OfferingId", offeringId);
                            insSch.Parameters.AddWithValue("@RoomId", validRoomId);
                            insSch.Parameters.AddWithValue("@DayOfWeek", sch.DayOfWeek <= 0 ? 1 : sch.DayOfWeek);
                            insSch.Parameters.AddWithValue("@StartTime", string.IsNullOrWhiteSpace(sch.StartTime) ? "09:00:00" : sch.StartTime);
                            insSch.Parameters.AddWithValue("@EndTime", string.IsNullOrWhiteSpace(sch.EndTime) ? "10:30:00" : sch.EndTime);
                            await insSch.ExecuteNonQueryAsync(cancellationToken);
                        }
                    }
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
            // 1. Delete CoursePrerequisites junctions
            await using (var delPrereqs = await CreateCommandAsync(
                "DELETE FROM CoursePrerequisites WHERE CourseId = @CourseId OR PrerequisiteCourseId = @CourseId;"))
            {
                delPrereqs.Parameters.AddWithValue("@CourseId", courseId);
                await delPrereqs.ExecuteNonQueryAsync(cancellationToken);
            }

            // 2. Delete CourseDepartments junctions
            await using (var delDepts = await CreateCommandAsync("DELETE FROM CourseDepartments WHERE CourseId = @CourseId;"))
            {
                delDepts.Parameters.AddWithValue("@CourseId", courseId);
                await delDepts.ExecuteNonQueryAsync(cancellationToken);
            }

            // 3. Find and delete schedules, instructors, and offerings
            const string getOffIdsSql = "SELECT CourseOfferingId FROM CourseOfferings WHERE CourseId = @CourseId;";
            var offIds = new List<int>();
            await using (var getOffCmd = await CreateCommandAsync(getOffIdsSql))
            {
                getOffCmd.Parameters.AddWithValue("@CourseId", courseId);
                await using var reader = await getOffCmd.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    offIds.Add(reader.GetInt32("CourseOfferingId"));
                }
            }

            foreach (var offId in offIds)
            {
                await using (var delSch = await CreateCommandAsync("DELETE FROM CourseSchedules WHERE CourseOfferingId = @OfferingId;"))
                {
                    delSch.Parameters.AddWithValue("@OfferingId", offId);
                    await delSch.ExecuteNonQueryAsync(cancellationToken);
                }

                await using (var delInst = await CreateCommandAsync("DELETE FROM CourseOfferingInstructors WHERE CourseOfferingId = @OfferingId;"))
                {
                    delInst.Parameters.AddWithValue("@OfferingId", offId);
                    await delInst.ExecuteNonQueryAsync(cancellationToken);
                }

                await using (var delOff = await CreateCommandAsync("DELETE FROM CourseOfferings WHERE CourseOfferingId = @OfferingId;"))
                {
                    delOff.Parameters.AddWithValue("@OfferingId", offId);
                    await delOff.ExecuteNonQueryAsync(cancellationToken);
                }
            }

            // 4. Delete Course record
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
                    GROUP_CONCAT(DISTINCT CONCAT(i.FirstName, ' ', i.LastName) SEPARATOR ', ') AS InstructorName,
                    c.CreatedAt,
                    c.UpdatedAt
                FROM CoursePrerequisites cp
                INNER JOIN Courses c ON cp.PrerequisiteCourseId = c.CourseId
                INNER JOIN CourseStatuses cs ON c.CourseStatusId = cs.CourseStatusId
                LEFT JOIN CourseDepartments cd ON c.CourseId = cd.CourseId
                LEFT JOIN Departments d ON cd.DepartmentId = d.DepartmentId
                LEFT JOIN CourseOfferings co ON c.CourseId = co.CourseId
                LEFT JOIN CourseOfferingInstructors coi ON co.CourseOfferingId = coi.CourseOfferingId
                LEFT JOIN Instructors i ON coi.InstructorId = i.InstructorId
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

    private async Task<int> EnsureValidRoomIdAsync(int requestedRoomId, CancellationToken cancellationToken)
    {
        const string checkSql = "SELECT RoomId FROM Rooms WHERE RoomId = @RoomId LIMIT 1;";
        await using (var checkCmd = await CreateCommandAsync(checkSql))
        {
            checkCmd.Parameters.AddWithValue("@RoomId", requestedRoomId);
            var res = await checkCmd.ExecuteScalarAsync(cancellationToken);
            if (res != null && res != DBNull.Value) return Convert.ToInt32(res);
        }

        const string anySql = "SELECT RoomId FROM Rooms ORDER BY RoomId ASC LIMIT 1;";
        await using (var anyCmd = await CreateCommandAsync(anySql))
        {
            var res = await anyCmd.ExecuteScalarAsync(cancellationToken);
            if (res != null && res != DBNull.Value) return Convert.ToInt32(res);
        }

        const string insSql = "INSERT IGNORE INTO Rooms (RoomId, BuildingName, RoomNumber, Capacity) VALUES (1, 'Main Hall', '101', 50);";
        await using (var insCmd = await CreateCommandAsync(insSql))
        {
            await insCmd.ExecuteNonQueryAsync(cancellationToken);
        }
        return 1;
    }

    private async Task<int> EnsureValidSemesterIdAsync(int requestedSemesterId, CancellationToken cancellationToken)
    {
        const string checkSql = "SELECT SemesterId FROM Semesters WHERE SemesterId = @SemesterId LIMIT 1;";
        await using (var checkCmd = await CreateCommandAsync(checkSql))
        {
            checkCmd.Parameters.AddWithValue("@SemesterId", requestedSemesterId);
            var res = await checkCmd.ExecuteScalarAsync(cancellationToken);
            if (res != null && res != DBNull.Value) return Convert.ToInt32(res);
        }

        const string anySql = "SELECT SemesterId FROM Semesters ORDER BY SemesterId ASC LIMIT 1;";
        await using (var anyCmd = await CreateCommandAsync(anySql))
        {
            var res = await anyCmd.ExecuteScalarAsync(cancellationToken);
            if (res != null && res != DBNull.Value) return Convert.ToInt32(res);
        }

        const string insSql = "INSERT IGNORE INTO Semesters (SemesterId, SemesterName, AcademicYear, StartDate, EndDate, IsCurrent) VALUES (1, 'Fall 2026', 2026, '2026-08-20', '2026-12-15', 1);";
        await using (var insCmd = await CreateCommandAsync(insSql))
        {
            await insCmd.ExecuteNonQueryAsync(cancellationToken);
        }
        return 1;
    }

    private async Task<int?> ValidateInstructorIdAsync(int requestedInstructorId, CancellationToken cancellationToken)
    {
        const string checkSql = "SELECT InstructorId FROM Instructors WHERE InstructorId = @InstructorId LIMIT 1;";
        await using (var checkCmd = await CreateCommandAsync(checkSql))
        {
            checkCmd.Parameters.AddWithValue("@InstructorId", requestedInstructorId);
            var res = await checkCmd.ExecuteScalarAsync(cancellationToken);
            if (res != null && res != DBNull.Value) return Convert.ToInt32(res);
        }

        const string anySql = "SELECT InstructorId FROM Instructors ORDER BY InstructorId ASC LIMIT 1;";
        await using (var anyCmd = await CreateCommandAsync(anySql))
        {
            var anyRes = await anyCmd.ExecuteScalarAsync(cancellationToken);
            if (anyRes != null && anyRes != DBNull.Value) return Convert.ToInt32(anyRes);
        }

        return null;
    }

    private async Task<int> EnsureValidDepartmentIdAsync(int requestedDepartmentId, CancellationToken cancellationToken)
    {
        const string checkSql = "SELECT DepartmentId FROM Departments WHERE DepartmentId = @DepartmentId LIMIT 1;";
        await using (var checkCmd = await CreateCommandAsync(checkSql))
        {
            checkCmd.Parameters.AddWithValue("@DepartmentId", requestedDepartmentId);
            var res = await checkCmd.ExecuteScalarAsync(cancellationToken);
            if (res != null && res != DBNull.Value) return Convert.ToInt32(res);
        }

        const string anySql = "SELECT DepartmentId FROM Departments ORDER BY DepartmentId ASC LIMIT 1;";
        await using (var anyCmd = await CreateCommandAsync(anySql))
        {
            var res = await anyCmd.ExecuteScalarAsync(cancellationToken);
            if (res != null && res != DBNull.Value) return Convert.ToInt32(res);
        }

        const string insSql = "INSERT IGNORE INTO Departments (DepartmentId, DepartmentCode, DepartmentName, CreatedAt) VALUES (1, 'CS', 'Computer Science', '2026-01-10');";
        await using (var insCmd = await CreateCommandAsync(insSql))
        {
            await insCmd.ExecuteNonQueryAsync(cancellationToken);
        }
        return 1;
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

    private async Task<IReadOnlyList<CourseOfferingDetailsDto>> GetOfferingsForCourseAsync(int courseId, CancellationToken cancellationToken)
    {
        var offerings = new List<CourseOfferingDetailsDto>();

        const string sql = @"
            SELECT 
                co.CourseOfferingId,
                co.CourseId,
                co.SemesterId,
                s.SemesterName,
                co.OfferingStatusId,
                os.StatusName AS OfferingStatusName,
                co.SectionNumber,
                co.Capacity
            FROM CourseOfferings co
            INNER JOIN Semesters s ON co.SemesterId = s.SemesterId
            INNER JOIN OfferingStatuses os ON co.OfferingStatusId = os.OfferingStatusId
            WHERE co.CourseId = @CourseId
            ORDER BY co.CourseOfferingId ASC;";

        await using (var command = await CreateCommandAsync(sql))
        {
            command.Parameters.AddWithValue("@CourseId", courseId);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                offerings.Add(new CourseOfferingDetailsDto
                {
                    CourseOfferingId = reader.GetInt32("CourseOfferingId"),
                    CourseId = reader.GetInt32("CourseId"),
                    SemesterId = reader.GetInt32("SemesterId"),
                    SemesterName = reader.GetString("SemesterName"),
                    OfferingStatusId = reader.GetInt32("OfferingStatusId"),
                    OfferingStatusName = reader.GetString("OfferingStatusName"),
                    SectionNumber = reader.GetString("SectionNumber"),
                    Capacity = reader.GetInt32("Capacity")
                });
            }
        }

        // Hydrate Instructors & Schedules for each offering
        for (int i = 0; i < offerings.Count; i++)
        {
            var off = offerings[i];
            var instructors = await GetInstructorsForOfferingAsync(off.CourseOfferingId, cancellationToken);
            var schedules = await GetSchedulesForOfferingAsync(off.CourseOfferingId, cancellationToken);

            offerings[i] = off with
            {
                Instructors = instructors,
                Schedules = schedules
            };
        }

        return offerings;
    }

    private async Task<IReadOnlyList<CourseOfferingInstructorDto>> GetInstructorsForOfferingAsync(int offeringId, CancellationToken cancellationToken)
    {
        var instructors = new List<CourseOfferingInstructorDto>();
        const string sql = @"
            SELECT 
                coi.InstructorId,
                CONCAT(i.FirstName, ' ', i.LastName) AS InstructorName,
                coi.IsPrimary
            FROM CourseOfferingInstructors coi
            INNER JOIN Instructors i ON coi.InstructorId = i.InstructorId
            WHERE coi.CourseOfferingId = @OfferingId;";

        await using var command = await CreateCommandAsync(sql);
        command.Parameters.AddWithValue("@OfferingId", offeringId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            instructors.Add(new CourseOfferingInstructorDto
            {
                InstructorId = reader.GetInt32("InstructorId"),
                InstructorName = reader.GetString("InstructorName"),
                IsPrimary = reader.GetBoolean("IsPrimary")
            });
        }

        return instructors;
    }

    private async Task<IReadOnlyList<CourseScheduleDto>> GetSchedulesForOfferingAsync(int offeringId, CancellationToken cancellationToken)
    {
        var schedules = new List<CourseScheduleDto>();
        const string sql = @"
            SELECT 
                cs.CourseScheduleId,
                cs.CourseOfferingId,
                cs.RoomId,
                r.RoomNumber,
                r.BuildingName,
                cs.DayOfWeek,
                CASE cs.DayOfWeek
                    WHEN 1 THEN 'Monday'
                    WHEN 2 THEN 'Tuesday'
                    WHEN 3 THEN 'Wednesday'
                    WHEN 4 THEN 'Thursday'
                    WHEN 5 THEN 'Friday'
                    WHEN 6 THEN 'Saturday'
                    WHEN 7 THEN 'Sunday'
                    ELSE 'Other'
                END AS DayName,
                cs.StartTime,
                cs.EndTime
            FROM CourseSchedules cs
            INNER JOIN Rooms r ON cs.RoomId = r.RoomId
            WHERE cs.CourseOfferingId = @OfferingId;";

        await using var command = await CreateCommandAsync(sql);
        command.Parameters.AddWithValue("@OfferingId", offeringId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            schedules.Add(new CourseScheduleDto
            {
                CourseScheduleId = reader.GetInt32("CourseScheduleId"),
                CourseOfferingId = reader.GetInt32("CourseOfferingId"),
                RoomId = reader.GetInt32("RoomId"),
                RoomNumber = reader.IsDBNull("RoomNumber") ? null : reader.GetString("RoomNumber"),
                BuildingName = reader.IsDBNull("BuildingName") ? null : reader.GetString("BuildingName"),
                DayOfWeek = reader.GetInt32("DayOfWeek"),
                DayName = reader.GetString("DayName"),
                StartTime = reader.GetTimeSpan("StartTime").ToString(@"hh\:mm"),
                EndTime = reader.GetTimeSpan("EndTime").ToString(@"hh\:mm")
            });
        }

        return schedules;
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
            InstructorName = reader.IsDBNull("InstructorName") ? null : reader.GetString("InstructorName"),
            CreatedAt = reader.GetDateTime("CreatedAt"),
            UpdatedAt = reader.GetDateTime("UpdatedAt")
        };
    }
}
