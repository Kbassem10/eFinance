using System.Data;
using MySqlConnector;
using StudentRegistrationPortal.Api.Data;
using StudentRegistrationPortal.Api.DTOs;

namespace StudentRegistrationPortal.Api.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ISqlDataAccess _db;

    public CourseRepository(ISqlDataAccess db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<IReadOnlyList<CourseDetailsDto>> GetAllCoursesAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT CourseId, CourseCode, CourseName, CreditHours, DifficultyLevel, StatusName, AssignedDepartments
            FROM vw_Courses
            ORDER BY CourseCode ASC;";

        return await _db.QueryAsync(sql, MapCourseDetailsDto, cancellationToken: cancellationToken);
    }

    public async Task<CourseDetailsDto?> GetCourseByIdAsync(int courseId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT CourseId, CourseCode, CourseName, CreditHours, DifficultyLevel, StatusName, AssignedDepartments
            FROM vw_Courses
            WHERE CourseId = @CourseId
            LIMIT 1;";

        var parameters = new[] { new MySqlParameter("@CourseId", courseId) };
        return await _db.QueryFirstOrDefaultAsync(sql, MapCourseDetailsDto, parameters, cancellationToken: cancellationToken);
    }

    public async Task<CourseDetailsDto?> GetCourseByCodeAsync(string courseCode, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT CourseId, CourseCode, CourseName, CreditHours, DifficultyLevel, StatusName, AssignedDepartments
            FROM vw_Courses
            WHERE CourseCode = @CourseCode
            LIMIT 1;";

        var parameters = new[] { new MySqlParameter("@CourseCode", courseCode) };
        return await _db.QueryFirstOrDefaultAsync(sql, MapCourseDetailsDto, parameters, cancellationToken: cancellationToken);
    }

    private static CourseDetailsDto MapCourseDetailsDto(MySqlDataReader reader)
    {
        return new CourseDetailsDto
        {
            CourseId = reader.GetSafeInt32("CourseId"),
            CourseCode = reader.GetSafeString("CourseCode"),
            CourseName = reader.GetSafeString("CourseName"),
            CreditHours = reader.GetSafeInt32("CreditHours"),
            DifficultyLevel = reader.GetSafeString("DifficultyLevel"),
            StatusName = reader.GetSafeString("StatusName"),
            AssignedDepartments = reader.GetSafeString("AssignedDepartments")
        };
    }
}
