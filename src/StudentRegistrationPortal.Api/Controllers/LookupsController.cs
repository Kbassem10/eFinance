using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentRegistrationPortal.Application.Common.Interfaces;
using StudentRegistrationPortal.Application.DTOs;

namespace StudentRegistrationPortal.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class LookupsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LookupsController> _logger;

    public LookupsController(IUnitOfWork unitOfWork, ILogger<LookupsController> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("departments")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<LookupItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDepartments(CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Lookups.GetDepartmentsAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("semesters")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<LookupItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSemesters(CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Lookups.GetSemestersAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("instructors")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<LookupItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInstructors(CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Lookups.GetInstructorsAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("rooms")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<LookupItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRooms(CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Lookups.GetRoomsAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("course-statuses")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<LookupItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseStatuses(CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Lookups.GetCourseStatusesAsync(cancellationToken);
        return Ok(result);
    }
}
