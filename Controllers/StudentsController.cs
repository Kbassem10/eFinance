using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using StudentRegistrationPortal.Api.DTOs;
using StudentRegistrationPortal.Api.Repositories;

namespace StudentRegistrationPortal.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IUnitOfWork unitOfWork, ILogger<StudentsController> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<StudentDetailsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var students = await _unitOfWork.Students.GetAllAsync();
        return Ok(students);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(StudentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);
        if (student == null)
        {
            return NotFound(new { message = $"Student with ID {id} not found." });
        }
        return Ok(student);
    }

    [HttpGet("by-number/{studentNumber}")]
    [ProducesResponseType(typeof(StudentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByStudentNumber(string studentNumber)
    {
        var student = await _unitOfWork.Students.GetByStudentNumberAsync(studentNumber);
        if (student == null)
        {
            return NotFound(new { message = $"Student with number '{studentNumber}' not found." });
        }
        return Ok(student);
    }

    [HttpPost]
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

            const int studentRoleId = 2; // Student Role
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
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentDto dto)
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
    public async Task<IActionResult> Delete(int id)
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
    public async Task<IActionResult> GetTotalCreditHours(int id, int semesterId)
    {
        int hours = await _unitOfWork.Students.GetTotalCreditHoursAsync(id, semesterId);
        return Ok(new { studentId = id, semesterId = semesterId, totalCreditHours = hours });
    }

    [HttpGet("login")]
    [ProducesResponseType(typeof(StudentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromQuery] string email, [FromQuery] string password, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
        if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var student = await _unitOfWork.Students.GetByUserIdAsync(user.UserId, cancellationToken);
        if (student == null)
        {
            return NotFound(new { message = "Student record not found for the provided email." });
        }

        return Ok(student);
    }
}
