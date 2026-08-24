using StudentRegistrationPortal.Application.Common.Interfaces;
using System.Data;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using StudentRegistrationPortal.Application.DTOs;

namespace StudentRegistrationPortal.Infrastructure.Persistence.Repositories;

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

    public async Task<StudentDetailsDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

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
                WHERE LOWER(Email) = LOWER(@Email)
                LIMIT 1;";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@Email", email.Trim());

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                return MapStudentFromReader(reader);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student with email '{Email}'", email);
            throw;
        }
    }

    public async Task<StudentDetailsDto?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = @"
                SELECT 
                    s.StudentId, s.StudentNumber, s.FirstName, s.MiddleName, s.LastName, s.FullName,
                    s.Email, s.NationalId, s.DateOfBirth, s.Gender, s.PhoneNumber, s.Address,
                    s.AdmissionDate, s.AcademicLevel, s.GPA, s.CompletedCreditHours,
                    s.DepartmentId, s.DepartmentName, s.DepartmentCode,
                    s.StudentStatusId, s.StatusName, s.CreatedAt, s.UpdatedAt
                FROM vw_Students s
                INNER JOIN Students st ON s.StudentId = st.StudentId
                WHERE st.UserId = @UserId
                LIMIT 1;";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                return MapStudentFromReader(reader);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student by user ID {UserId}", userId);
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

    public async Task<IReadOnlyList<EnrollmentResultDto>> EnrollInCoursesAsync(int studentId, IReadOnlyList<int> courseIds, CancellationToken cancellationToken = default)
    {
        var results = new List<EnrollmentResultDto>();
        if (courseIds == null || courseIds.Count == 0) return results;

        try
        {
            // Ensure status 5 (Pending) exists in EnrollmentStatuses
            await using (var statusCmd = await CreateCommandAsync(
                "INSERT IGNORE INTO EnrollmentStatuses (EnrollmentStatusId, StatusName, Description) VALUES (5, 'Pending', 'Pending enrollment approval');"))
            {
                await statusCmd.ExecuteNonQueryAsync(cancellationToken);
            }

            foreach (var courseId in courseIds.Distinct())
            {
                // Find or resolve CourseOfferingId for this course
                int offeringId = 0;
                const string findOfferingSql = @"
                    SELECT co.CourseOfferingId 
                    FROM CourseOfferings co 
                    INNER JOIN Semesters s ON co.SemesterId = s.SemesterId 
                    WHERE co.CourseId = @CourseId AND s.IsCurrent = 1 
                    LIMIT 1;";

                await using (var findCmd = await CreateCommandAsync(findOfferingSql))
                {
                    findCmd.Parameters.AddWithValue("@CourseId", courseId);
                    var scalar = await findCmd.ExecuteScalarAsync(cancellationToken);
                    if (scalar != null && scalar != DBNull.Value)
                    {
                        offeringId = Convert.ToInt32(scalar);
                    }
                }

                if (offeringId == 0)
                {
                    // Fallback to any offering for this course
                    await using var fallbackCmd = await CreateCommandAsync("SELECT CourseOfferingId FROM CourseOfferings WHERE CourseId = @CourseId LIMIT 1;");
                    fallbackCmd.Parameters.AddWithValue("@CourseId", courseId);
                    var scalar = await fallbackCmd.ExecuteScalarAsync(cancellationToken);
                    if (scalar != null && scalar != DBNull.Value)
                    {
                        offeringId = Convert.ToInt32(scalar);
                    }
                }

                if (offeringId == 0)
                {
                    // Create default offering for the course
                    const string createOfferingSql = @"
                        INSERT INTO CourseOfferings (CourseId, SemesterId, OfferingStatusId, SectionNumber, Capacity, CreatedAt)
                        VALUES (@CourseId, (SELECT SemesterId FROM Semesters ORDER BY SemesterId DESC LIMIT 1), 1, 'SEC-01', 30, UTC_TIMESTAMP());
                        SELECT LAST_INSERT_ID();";

                    await using var createOfferingCmd = await CreateCommandAsync(createOfferingSql);
                    createOfferingCmd.Parameters.AddWithValue("@CourseId", courseId);
                    var newOffId = await createOfferingCmd.ExecuteScalarAsync(cancellationToken);
                    offeringId = Convert.ToInt32(newOffId);
                }

                // Check if already enrolled in this offering
                const string checkExistingSql = @"
                    SELECT EnrollmentId FROM Enrollments 
                    WHERE StudentId = @StudentId AND CourseOfferingId = @OfferingId 
                    LIMIT 1;";

                int existingEnrollmentId = 0;
                await using (var checkCmd = await CreateCommandAsync(checkExistingSql))
                {
                    checkCmd.Parameters.AddWithValue("@StudentId", studentId);
                    checkCmd.Parameters.AddWithValue("@OfferingId", offeringId);
                    var exObj = await checkCmd.ExecuteScalarAsync(cancellationToken);
                    if (exObj != null && exObj != DBNull.Value)
                    {
                        existingEnrollmentId = Convert.ToInt32(exObj);
                    }
                }

                int enrollmentId;
                if (existingEnrollmentId == 0)
                {
                    const string insertEnrollmentSql = @"
                        INSERT INTO Enrollments (StudentId, CourseOfferingId, EnrollmentStatusId, RegistrationDate)
                        VALUES (@StudentId, @OfferingId, 5, UTC_TIMESTAMP());
                        SELECT LAST_INSERT_ID();";

                    await using var insCmd = await CreateCommandAsync(insertEnrollmentSql);
                    insCmd.Parameters.AddWithValue("@StudentId", studentId);
                    insCmd.Parameters.AddWithValue("@OfferingId", offeringId);
                    var insId = await insCmd.ExecuteScalarAsync(cancellationToken);
                    enrollmentId = Convert.ToInt32(insId);
                }
                else
                {
                    enrollmentId = existingEnrollmentId;
                }

                // Fetch course info
                string courseCode = "";
                string courseName = "";
                await using (var infoCmd = await CreateCommandAsync("SELECT CourseCode, CourseName FROM Courses WHERE CourseId = @CourseId LIMIT 1;"))
                {
                    infoCmd.Parameters.AddWithValue("@CourseId", courseId);
                    await using var reader = await infoCmd.ExecuteReaderAsync(cancellationToken);
                    if (await reader.ReadAsync(cancellationToken))
                    {
                        courseCode = reader.GetString("CourseCode");
                        courseName = reader.GetString("CourseName");
                    }
                }

                results.Add(new EnrollmentResultDto
                {
                    EnrollmentId = enrollmentId,
                    StudentId = studentId,
                    CourseId = courseId,
                    CourseCode = courseCode,
                    CourseName = courseName,
                    Status = "Pending",
                    RegistrationDate = DateTime.UtcNow
                });
            }

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enrolling student ID {StudentId}", studentId);
            throw;
        }
    }

    public async Task<IReadOnlyList<EnrollmentResultDto>> GetStudentEnrollmentsAsync(int studentId, CancellationToken cancellationToken = default)
    {
        var list = new List<EnrollmentResultDto>();
        try
        {
            const string sql = @"
                SELECT 
                    e.EnrollmentId,
                    e.StudentId,
                    c.CourseId,
                    c.CourseCode,
                    c.CourseName,
                    es.StatusName AS Status,
                    e.RegistrationDate
                FROM Enrollments e
                INNER JOIN CourseOfferings co ON e.CourseOfferingId = co.CourseOfferingId
                INNER JOIN Courses c ON co.CourseId = c.CourseId
                INNER JOIN EnrollmentStatuses es ON e.EnrollmentStatusId = es.EnrollmentStatusId
                WHERE e.StudentId = @StudentId
                ORDER BY e.EnrollmentId DESC;";

            await using var command = await CreateCommandAsync(sql);
            command.Parameters.AddWithValue("@StudentId", studentId);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                list.Add(new EnrollmentResultDto
                {
                    EnrollmentId = reader.GetInt32("EnrollmentId"),
                    StudentId = reader.GetInt32("StudentId"),
                    CourseId = reader.GetInt32("CourseId"),
                    CourseCode = reader.GetString("CourseCode"),
                    CourseName = reader.GetString("CourseName"),
                    Status = reader.GetString("Status"),
                    RegistrationDate = reader.GetDateTime("RegistrationDate")
                });
            }

            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enrollments for student ID {StudentId}", studentId);
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
                    InstructorName = reader.IsDBNull("InstructorName") ? null : reader.GetString("InstructorName"),
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
