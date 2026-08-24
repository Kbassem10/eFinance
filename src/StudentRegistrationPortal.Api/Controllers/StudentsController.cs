using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentRegistrationPortal.Application.Common.Interfaces;
using StudentRegistrationPortal.Application.DTOs;

namespace StudentRegistrationPortal.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        ILogger<StudentsController> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<StudentDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll()
    {
        var students = await _unitOfWork.Students.GetAllAsync();
        return Ok(students);
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(StudentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int currentUserId))
        {
            return Unauthorized(new { message = "Invalid token claims." });
        }

        var student = await _unitOfWork.Students.GetByUserIdAsync(currentUserId, cancellationToken);
        if (student == null)
        {
            return NotFound(new { message = "No student profile associated with your account." });
        }

        return Ok(student);
    }

    [HttpGet("me/enrollments")]
    [ProducesResponseType(typeof(IReadOnlyList<EnrollmentResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyEnrollments(CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int currentUserId))
        {
            return Unauthorized(new { message = "Invalid token claims." });
        }

        var student = await _unitOfWork.Students.GetByUserIdAsync(currentUserId, cancellationToken);
        if (student == null)
        {
            return NotFound(new { message = "No student profile associated with your account." });
        }

        var list = await _unitOfWork.Students.GetStudentEnrollmentsAsync(student.StudentId, cancellationToken);
        return Ok(list);
    }

    [HttpPost("me/enrollments")]
    [ProducesResponseType(typeof(IReadOnlyList<EnrollmentResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EnrollInCourses([FromBody] EnrollCoursesRequestDto dto, CancellationToken cancellationToken)
    {
        if (dto == null || dto.CourseIds == null || dto.CourseIds.Count == 0)
        {
            return BadRequest(new { message = "Please provide at least one course ID to enroll." });
        }

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int currentUserId))
        {
            return Unauthorized(new { message = "Invalid token claims." });
        }

        var student = await _unitOfWork.Students.GetByUserIdAsync(currentUserId, cancellationToken);
        if (student == null)
        {
            return NotFound(new { message = "No student profile associated with your account." });
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var enrollments = await _unitOfWork.Students.EnrollInCoursesAsync(student.StudentId, dto.CourseIds, cancellationToken);

            await _unitOfWork.CommitAsync();

            return Ok(enrollments);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Failed to enroll student ID {StudentId}.", student.StudentId);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(StudentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int currentUserId))
        {
            return Unauthorized(new { message = "Invalid token claims." });
        }

        bool isAdmin = User.IsInRole("Admin") || User.IsInRole("Registrar");

        var student = await _unitOfWork.Students.GetByIdAsync(id);
        if (student == null)
        {
            return NotFound(new { message = $"Student with ID {id} not found." });
        }

        // Enforce Ownership: Regular students can only access their own student record
        if (!isAdmin)
        {
            var myStudentProfile = await _unitOfWork.Students.GetByUserIdAsync(currentUserId, cancellationToken);
            if (myStudentProfile == null || myStudentProfile.StudentId != id)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Forbidden: You are not authorized to view data for another user." });
            }
        }

        return Ok(student);
    }

    [HttpGet("by-number/{studentNumber}")]
    [ProducesResponseType(typeof(StudentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByStudentNumber([FromRoute] string studentNumber)
    {
        var student = await _unitOfWork.Students.GetByStudentNumberAsync(studentNumber);
        if (student == null)
        {
            return NotFound(new { message = $"Student with number '{studentNumber}' not found." });
        }
        return Ok(student);
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(StudentDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto dto, CancellationToken cancellationToken)
    {
        if (dto == null)
        {
            return BadRequest(new { message = "Invalid student payload." });
        }

        var existingUser = await _unitOfWork.Users.GetByEmailAsync(dto.Email, cancellationToken);
        if (existingUser != null)
        {
            return Conflict(new { message = $"A user with email '{dto.Email}' already exists." });
        }

        var existingStudent = await _unitOfWork.Students.GetByStudentNumberAsync(dto.StudentNumber);
        if (existingStudent != null)
        {
            return Conflict(new { message = $"A student with student number '{dto.StudentNumber}' already exists." });
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password, 11);
            int newUserId = await _unitOfWork.Users.CreateAsync(dto.Email, passwordHash, cancellationToken);

            const int studentRoleId = 3; // Student Role
            await _unitOfWork.Users.AssignRoleAsync(newUserId, studentRoleId, cancellationToken);

            int newStudentId = await _unitOfWork.Students.CreateAsync(newUserId, dto, cancellationToken);
            var createdStudent = await _unitOfWork.Students.GetByIdAsync(newStudentId);

            await _unitOfWork.CommitAsync();

            return CreatedAtAction(nameof(GetById), new { id = newStudentId, version = "1.0" }, createdStudent);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Failed to create student.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStudentDto dto)
    {
        if (dto == null)
        {
            return BadRequest(new { message = "Invalid student payload." });
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            bool updated = await _unitOfWork.Students.UpdateAsync(id, dto);
            if (!updated)
            {
                await _unitOfWork.RollbackAsync();
                return NotFound(new { message = $"Student with ID {id} not found." });
            }

            await _unitOfWork.CommitAsync();
            return Ok(new { message = $"Student with ID {id} updated successfully." });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Failed to update student ID {StudentId}.", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            bool deleted = await _unitOfWork.Students.DeleteAsync(id);
            if (!deleted)
            {
                await _unitOfWork.RollbackAsync();
                return NotFound(new { message = $"Student with ID {id} not found." });
            }

            await _unitOfWork.CommitAsync();
            return Ok(new { message = $"Student with ID {id} deleted successfully." });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Failed to delete student ID {StudentId}.", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}/credit-hours/{semesterId:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTotalCreditHours([FromRoute] int id, [FromRoute] int semesterId)
    {
        int hours = await _unitOfWork.Students.GetTotalCreditHoursAsync(id, semesterId);
        return Ok(new { studentId = id, semesterId = semesterId, totalCreditHours = hours });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken cancellationToken)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email, cancellationToken);
        if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(dto.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var roleIds = await _unitOfWork.Users.GetUserRoleIdsAsync(user.UserId, cancellationToken);
        if (!roleIds.Contains(3) && !roleIds.Contains(2))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied. User does not have Student permissions (RoleId 3)." });
        }

        var student = await _unitOfWork.Students.GetByUserIdAsync(user.UserId, cancellationToken);
        if (student == null)
        {
            return NotFound(new { message = "Student record not found for the authenticated user." });
        }

        var token = _jwtTokenService.GenerateToken(user, "Student", student.StudentId);
        var expiresAt = DateTime.UtcNow.AddMinutes(120);

        return Ok(new LoginResponseDto(
            Token: token,
            TokenType: "Bearer",
            ExpiresAt: expiresAt,
            Student: student
        ));
    }
}
