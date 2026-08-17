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

    /// Retrieves a single student by primary key ID.
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

    /// Retrieves a student by their unique student number.
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

    /// Invokes the sp_ManageStudent stored procedure in INSERT mode.
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto dto, CancellationToken cancellationToken)
    {
        if (dto == null)
        {
            return BadRequest(new { message = "Invalid student payload." });
        }

        var result = await _studentRepository.CreateViaStoredProcedureAsync(dto, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ProcessingMessage });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.AffectedId }, result);
    }

    /// Invokes the sp_ManageStudent stored procedure in UPDATE mode.
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentDto dto, CancellationToken cancellationToken)
    {
        if (dto == null)
        {
            return BadRequest(new { message = "Invalid student payload." });
        }

        var result = await _studentRepository.UpdateViaStoredProcedureAsync(id, dto, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ProcessingMessage });
        }

        return Ok(result);
    }

    /// Invokes the sp_ManageStudent stored procedure in DELETE mode.
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _studentRepository.DeleteViaStoredProcedureAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ProcessingMessage });
        }

        return Ok(result);
    }

    /// Executes the fn_GetStudentTotalCreditHours scalar function via ADO.NET.
    [HttpGet("{id:int}/credit-hours/{semesterId:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTotalCreditHours(int id, int semesterId, CancellationToken cancellationToken)
    {
        int hours = await _studentRepository.GetTotalCreditHoursAsync(id, semesterId, cancellationToken);
        return Ok(new { studentId = id, semesterId = semesterId, totalCreditHours = hours });
    }
}
