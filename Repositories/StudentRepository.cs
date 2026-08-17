using System.Data;
using MySqlConnector;
using StudentRegistrationPortal.Api.Data;
using StudentRegistrationPortal.Api.DTOs;
using StudentRegistrationPortal.Api.Entities;

namespace StudentRegistrationPortal.Api.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ISqlDataAccess _db;

    public StudentRepository(ISqlDataAccess db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<IReadOnlyList<StudentDetailsDto>> GetAllAsync(CancellationToken cancellationToken = default)
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

        return await _db.QueryAsync(sql, MapStudentDetailsDto, cancellationToken: cancellationToken);
    }

    public async Task<StudentDetailsDto?> GetByIdAsync(int studentId, CancellationToken cancellationToken = default)
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

        var parameters = new[] { new MySqlParameter("@StudentId", studentId) };
        return await _db.QueryFirstOrDefaultAsync(sql, MapStudentDetailsDto, parameters, cancellationToken: cancellationToken);
    }

    public async Task<StudentDetailsDto?> GetByStudentNumberAsync(string studentNumber, CancellationToken cancellationToken = default)
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

        var parameters = new[] { new MySqlParameter("@StudentNumber", studentNumber) };
        return await _db.QueryFirstOrDefaultAsync(sql, MapStudentDetailsDto, parameters, cancellationToken: cancellationToken);
    }

    public async Task<StoredProcedureResult> CreateViaStoredProcedureAsync(CreateStudentDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var parameters = new List<MySqlParameter>
        {
            new("p_ActionType", "INSERT"),
            new("p_StudentId", MySqlDbType.Int32) { Direction = ParameterDirection.InputOutput, Value = 0 },
            new("p_UserId", dto.UserId),
            new("p_DepartmentId", dto.DepartmentId),
            new("p_StudentStatusId", dto.StudentStatusId),
            new("p_StudentNumber", dto.StudentNumber),
            new("p_FirstName", dto.FirstName),
            new("p_LastName", dto.LastName),
            new("p_NationalId", dto.NationalId),
            new("p_AdmissionDate", dto.AdmissionDate.ToDateTime(TimeOnly.MinValue)),
            new("p_ProcessingStatus", MySqlDbType.Int32) { Direction = ParameterDirection.Output },
            new("p_ProcessingMessage", MySqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
        };

        return await _db.ExecuteStoredProcedureWithStatusAsync(
            "sp_ManageStudent",
            parameters,
            statusParamName: "p_ProcessingStatus",
            messageParamName: "p_ProcessingMessage",
            idInOutParamName: "p_StudentId",
            cancellationToken: cancellationToken);
    }

    public async Task<StoredProcedureResult> UpdateViaStoredProcedureAsync(int studentId, UpdateStudentDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var parameters = new List<MySqlParameter>
        {
            new("p_ActionType", "UPDATE"),
            new("p_StudentId", MySqlDbType.Int32) { Direction = ParameterDirection.InputOutput, Value = studentId },
            new("p_UserId", DBNull.Value),
            new("p_DepartmentId", dto.DepartmentId),
            new("p_StudentStatusId", dto.StudentStatusId),
            new("p_StudentNumber", DBNull.Value),
            new("p_FirstName", dto.FirstName),
            new("p_LastName", dto.LastName),
            new("p_NationalId", DBNull.Value),
            new("p_AdmissionDate", dto.AdmissionDate.ToDateTime(TimeOnly.MinValue)),
            new("p_ProcessingStatus", MySqlDbType.Int32) { Direction = ParameterDirection.Output },
            new("p_ProcessingMessage", MySqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
        };

        return await _db.ExecuteStoredProcedureWithStatusAsync(
            "sp_ManageStudent",
            parameters,
            statusParamName: "p_ProcessingStatus",
            messageParamName: "p_ProcessingMessage",
            idInOutParamName: "p_StudentId",
            cancellationToken: cancellationToken);
    }

    public async Task<StoredProcedureResult> DeleteViaStoredProcedureAsync(int studentId, CancellationToken cancellationToken = default)
    {
        var parameters = new List<MySqlParameter>
        {
            new("p_ActionType", "DELETE"),
            new("p_StudentId", MySqlDbType.Int32) { Direction = ParameterDirection.InputOutput, Value = studentId },
            new("p_UserId", DBNull.Value),
            new("p_DepartmentId", 0),
            new("p_StudentStatusId", 0),
            new("p_StudentNumber", string.Empty),
            new("p_FirstName", string.Empty),
            new("p_LastName", string.Empty),
            new("p_NationalId", string.Empty),
            new("p_AdmissionDate", DateTime.UtcNow),
            new("p_ProcessingStatus", MySqlDbType.Int32) { Direction = ParameterDirection.Output },
            new("p_ProcessingMessage", MySqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
        };

        return await _db.ExecuteStoredProcedureWithStatusAsync(
            "sp_ManageStudent",
            parameters,
            statusParamName: "p_ProcessingStatus",
            messageParamName: "p_ProcessingMessage",
            idInOutParamName: "p_StudentId",
            cancellationToken: cancellationToken);
    }

    public async Task<int> GetTotalCreditHoursAsync(int studentId, int semesterId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT fn_GetStudentTotalCreditHours(@StudentId, @SemesterId);";
        var parameters = new[]
        {
            new MySqlParameter("@StudentId", studentId),
            new MySqlParameter("@SemesterId", semesterId)
        };

        return await _db.ExecuteScalarAsync<int>(sql, parameters, cancellationToken: cancellationToken);
    }

    private static StudentDetailsDto MapStudentDetailsDto(MySqlDataReader reader)
    {
        return new StudentDetailsDto
        {
            StudentId = reader.GetSafeInt32("StudentId"),
            StudentNumber = reader.GetSafeString("StudentNumber"),
            FirstName = reader.GetSafeString("FirstName"),
            MiddleName = reader.GetSafeNullableString("MiddleName"),
            LastName = reader.GetSafeString("LastName"),
            FullName = reader.GetSafeString("FullName"),
            Email = reader.GetSafeString("Email"),
            NationalId = reader.GetSafeNullableString("NationalId"),
            DateOfBirth = reader.GetSafeDateOnly("DateOfBirth"),
            Gender = reader.GetSafeNullableString("Gender"),
            PhoneNumber = reader.GetSafeNullableString("PhoneNumber"),
            Address = reader.GetSafeNullableString("Address"),
            AdmissionDate = reader.GetSafeDateOnly("AdmissionDate"),
            AcademicLevel = reader.GetSafeInt32("AcademicLevel"),
            GPA = reader.GetSafeDecimal("GPA"),
            CompletedCreditHours = reader.GetSafeInt32("CompletedCreditHours"),
            DepartmentId = reader.GetSafeInt32("DepartmentId"),
            DepartmentName = reader.GetSafeString("DepartmentName"),
            DepartmentCode = reader.GetSafeString("DepartmentCode"),
            StudentStatusId = reader.GetSafeInt32("StudentStatusId"),
            StatusName = reader.GetSafeString("StatusName"),
            CreatedAt = reader.GetSafeDateTime("CreatedAt"),
            UpdatedAt = reader.GetSafeDateTime("UpdatedAt")
        };
    }
}
