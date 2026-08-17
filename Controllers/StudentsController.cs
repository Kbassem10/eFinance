using Microsoft.AspNetCore.Mvc;
using StudentRegistrationPortal.Api.DTOs;
using StudentRegistrationPortal.Api.Repositories;

namespace StudentRegistrationPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentRepository _studentRepository;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IStudentRepository studentRepository, ILogger<StudentsController> logger)
    {
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<StudentDetailsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var students = await _studentRepository.GetAllAsync(cancellationToken);
        return Ok(students);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(StudentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(id, cancellationToken);
        if (student == null)
        {
            return NotFound(new { message = $"Student with ID {id} not found." });
        }
        return Ok(student);
    }

    [HttpGet("by-number/{studentNumber}")]
    [ProducesResponseType(typeof(StudentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByStudentNumber(string studentNumber, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByStudentNumberAsync(studentNumber, cancellationToken);
        if (student == null)
        {
            return NotFound(new { message = $"Student with number '{studentNumber}' not found." });
        }
        return Ok(student);
    }

    [HttpPost]
    [ProducesResponseType(typeof(StudentDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto dto, CancellationToken cancellationToken)
    {
        if (dto == null)
        {
            return BadRequest(new { message = "Invalid student payload." });
        }

        try
        {
            int newStudentId = await _studentRepository.CreateAsync(dto, cancellationToken);
            var createdStudent = await _studentRepository.GetByIdAsync(newStudentId, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = newStudentId }, createdStudent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create student.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentDto dto, CancellationToken cancellationToken)
    {
        if (dto == null)
        {
            return BadRequest(new { message = "Invalid student payload." });
        }

        try
        {
            bool updated = await _studentRepository.UpdateAsync(id, dto, cancellationToken);
            if (!updated)
            {
                return NotFound(new { message = $"Student with ID {id} not found." });
            }

            return Ok(new { message = $"Student with ID {id} updated successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update student ID {StudentId}.", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            bool deleted = await _studentRepository.DeleteAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound(new { message = $"Student with ID {id} not found." });
            }

            return Ok(new { message = $"Student with ID {id} deleted successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete student ID {StudentId}.", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}/credit-hours/{semesterId:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTotalCreditHours(int id, int semesterId, CancellationToken cancellationToken)
    {
        int hours = await _studentRepository.GetTotalCreditHoursAsync(id, semesterId, cancellationToken);
        return Ok(new { studentId = id, semesterId = semesterId, totalCreditHours = hours });
    }
}
