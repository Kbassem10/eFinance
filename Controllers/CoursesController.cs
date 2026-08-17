using Microsoft.AspNetCore.Mvc;
using StudentRegistrationPortal.Api.DTOs;
using StudentRegistrationPortal.Api.Repositories;

namespace StudentRegistrationPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseRepository _courseRepository;

    public CoursesController(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CourseDetailsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var courses = await _courseRepository.GetAllCoursesAsync(cancellationToken);
        return Ok(courses);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CourseDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetCourseByIdAsync(id, cancellationToken);
        if (course == null)
        {
            return NotFound(new { message = $"Course with ID {id} not found." });
        }
        return Ok(course);
    }
}
