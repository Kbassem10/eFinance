using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentRegistrationPortal.Api.DTOs;
using StudentRegistrationPortal.Api.Entities;
using StudentRegistrationPortal.Api.Repositories;
using StudentRegistrationPortal.Api.Services;

namespace StudentRegistrationPortal.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        ILogger<AdminController> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AdminLoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
        if (!roleIds.Contains(1))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: User does not have Admin permissions (RoleId 1)." });
        }

        var userRoles = await _unitOfWork.Users.GetUserRolesAsync(user.UserId, cancellationToken);
        var roleNames = userRoles.Select(r => r.RoleName).ToList();

        var token = _jwtTokenService.GenerateToken(user, "Admin");
        var expiresAt = DateTime.UtcNow.AddMinutes(120);

        var adminDto = new AdminDetailsDto
        {
            UserId = user.UserId,
            Email = user.Email,
            IsActive = user.IsActive,
            Roles = roleNames,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return Ok(new AdminLoginResponseDto(
            Token: token,
            TokenType: "Bearer",
            ExpiresAt: expiresAt,
            Admin: adminDto
        ));
    }

    [HttpGet("me")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AdminDetailsDto), StatusCodes.Status200OK)]
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

        var user = await _unitOfWork.Users.GetByIdAsync(currentUserId, cancellationToken);
        if (user == null)
        {
            return NotFound(new { message = "Admin user not found." });
        }

        var userRoles = await _unitOfWork.Users.GetUserRolesAsync(user.UserId, cancellationToken);
        var adminDto = new AdminDetailsDto
        {
            UserId = user.UserId,
            Email = user.Email,
            IsActive = user.IsActive,
            Roles = userRoles.Select(r => r.RoleName).ToList(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return Ok(adminDto);
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IReadOnlyList<AdminDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        var result = new List<AdminDetailsDto>();

        foreach (var user in users)
        {
            var userRoles = await _unitOfWork.Users.GetUserRolesAsync(user.UserId, cancellationToken);
            result.Add(new AdminDetailsDto
            {
                UserId = user.UserId,
                Email = user.Email,
                IsActive = user.IsActive,
                Roles = userRoles.Select(r => r.RoleName).ToList(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            });
        }

        return Ok(result);
    }

    [HttpPost("users/{userId:int}/roles/{roleId:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignRole([FromRoute] int userId, [FromRoute] int roleId, CancellationToken cancellationToken)
    {
        await _unitOfWork.Users.AssignRoleAsync(userId, roleId, cancellationToken);
        return Ok(new { message = $"Role ID {roleId} assigned to User ID {userId} successfully." });
    }
}
