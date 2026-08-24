using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentRegistrationPortal.Application.DTOs;
using StudentRegistrationPortal.Application.Common.Interfaces;

namespace StudentRegistrationPortal.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(IUnitOfWork unitOfWork, ILogger<CoursesController> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<CourseDetailsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? departmentId,
        [FromQuery] int? statusId,
        CancellationToken cancellationToken)
    {
        var courses = await _unitOfWork.Courses.GetAllAsync(departmentId, statusId, cancellationToken);
        return Ok(courses);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CourseDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id, cancellationToken);
        if (course == null)
        {
            return NotFound(new { message = $"Course with ID {id} not found." });
        }
        return Ok(course);
    }

    [HttpGet("code/{courseCode}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CourseDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode([FromRoute] string courseCode, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByCourseCodeAsync(courseCode, cancellationToken);
        if (course == null)
        {
            return NotFound(new { message = $"Course with code '{courseCode}' not found." });
        }
        return Ok(course);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Registrar")]
    [ProducesResponseType(typeof(CourseDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateCourseDto dto, CancellationToken cancellationToken)
    {
        if (dto == null)
        {
            return BadRequest(new { message = "Invalid course payload." });
        }

        var existing = await _unitOfWork.Courses.GetByCourseCodeAsync(dto.CourseCode, cancellationToken);
        if (existing != null)
        {
            return Conflict(new { message = $"A course with code '{dto.CourseCode}' already exists." });
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            int newCourseId = await _unitOfWork.Courses.CreateAsync(dto, cancellationToken);
            var createdCourse = await _unitOfWork.Courses.GetByIdAsync(newCourseId, cancellationToken);

            await _unitOfWork.CommitAsync();

            return CreatedAtAction(nameof(GetById), new { id = newCourseId, version = "1.0" }, createdCourse);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Failed to create course.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Registrar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] UpdateCourseDto dto,
        CancellationToken cancellationToken)
    {
        if (dto == null)
        {
            return BadRequest(new { message = "Invalid course payload." });
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            bool updated = await _unitOfWork.Courses.UpdateAsync(id, dto, cancellationToken);
            if (!updated)
            {
                await _unitOfWork.RollbackAsync();
                return NotFound(new { message = $"Course with ID {id} not found." });
            }

            await _unitOfWork.CommitAsync();
            return Ok(new { message = $"Course with ID {id} updated successfully." });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Failed to update course ID {CourseId}.", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Registrar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            bool deleted = await _unitOfWork.Courses.DeleteAsync(id, cancellationToken);
            if (!deleted)
            {
                await _unitOfWork.RollbackAsync();
                return NotFound(new { message = $"Course with ID {id} not found." });
            }

            await _unitOfWork.CommitAsync();
            return Ok(new { message = $"Course with ID {id} deleted successfully." });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Failed to delete course ID {CourseId}.", id);
            return BadRequest(new { message = ex.Message });
        }
    }
}

