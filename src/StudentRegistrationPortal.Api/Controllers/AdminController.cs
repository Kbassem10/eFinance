using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentRegistrationPortal.Application.Common.Interfaces;
using StudentRegistrationPortal.Application.DTOs;
using StudentRegistrationPortal.Domain.Entities;

namespace StudentRegistrationPortal.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IUnitOfWork unitOfWork,
        ILogger<AdminController> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("me")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int currentUserId))
        {
            return Unauthorized(new { message = "Invalid token claims." });
        }

        var user = await _unitOfWork.Auth.GetUserDetailsByIdAsync(currentUserId, cancellationToken);
        if (user == null)
        {
            return NotFound(new { message = "Admin user not found." });
        }

        return Ok(user);
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IReadOnlyList<UserDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Admin.GetAllUsersAsync(cancellationToken);
        var result = new List<UserDetailsDto>();

        foreach (var user in users)
        {
            var userRoles = await _unitOfWork.Auth.GetUserRolesAsync(user.UserId, cancellationToken);
            var student = await _unitOfWork.Auth.GetStudentByUserIdAsync(user.UserId, cancellationToken);
            result.Add(new UserDetailsDto
            {
                UserId = user.UserId,
                Email = user.Email,
                IsActive = user.IsActive,
                Roles = userRoles.Select(r => r.RoleName).ToList(),
                StudentId = student?.StudentId,
                StudentNumber = student?.StudentNumber,
                FullName = student?.FullName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            });
        }

        return Ok(result);
    }

    [HttpGet("enrollments")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IReadOnlyList<AdminEnrollmentDetailsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEnrollments([FromQuery] int? statusId, CancellationToken cancellationToken)
    {
        var list = await _unitOfWork.Admin.GetAllEnrollmentsAsync(statusId, cancellationToken);
        return Ok(list);
    }

    [HttpPut("enrollments/{id:int}/approve")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveEnrollment([FromRoute] int id, CancellationToken cancellationToken)
    {
        const int enrolledStatusId = 1; // Enrolled
        bool updated = await _unitOfWork.Admin.UpdateEnrollmentStatusAsync(id, enrolledStatusId, cancellationToken);
        if (!updated)
        {
            return NotFound(new { message = $"Enrollment record with ID {id} not found." });
        }
        return Ok(new { message = $"Enrollment ID {id} approved successfully (Status: Enrolled)." });
    }

    [HttpPut("enrollments/{id:int}/decline")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeclineEnrollment([FromRoute] int id, CancellationToken cancellationToken)
    {
        const int droppedStatusId = 3; // Dropped / Declined
        bool updated = await _unitOfWork.Admin.UpdateEnrollmentStatusAsync(id, droppedStatusId, cancellationToken);
        if (!updated)
        {
            return NotFound(new { message = $"Enrollment record with ID {id} not found." });
        }
        return Ok(new { message = $"Enrollment ID {id} declined successfully (Status: Dropped/Declined)." });
    }

    [HttpPut("enrollments/{id:int}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnrollmentStatus([FromRoute] int id, [FromBody] UpdateEnrollmentStatusDto dto, CancellationToken cancellationToken)
    {
        if (dto == null || dto.EnrollmentStatusId <= 0)
        {
            return BadRequest(new { message = "Valid EnrollmentStatusId is required." });
        }

        bool updated = await _unitOfWork.Admin.UpdateEnrollmentStatusAsync(id, dto.EnrollmentStatusId, cancellationToken);
        if (!updated)
        {
            return NotFound(new { message = $"Enrollment record with ID {id} not found." });
        }
        return Ok(new { message = $"Enrollment ID {id} updated to Status ID {dto.EnrollmentStatusId} successfully." });
    }

    [HttpGet("users/{userId:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById([FromRoute] int userId, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Auth.GetUserDetailsByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return NotFound(new { message = $"User with ID {userId} was not found." });
        }
        return Ok(user);
    }
}
