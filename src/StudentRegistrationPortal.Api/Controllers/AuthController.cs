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
public class AuthController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        ILogger<AuthController> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken cancellationToken)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        var user = await _unitOfWork.Auth.GetUserByEmailAsync(dto.Email, cancellationToken);
        if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(dto.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var roleIds = await _unitOfWork.Auth.GetUserRoleIdsAsync(user.UserId, cancellationToken);

        string userRole;
        if (roleIds.Contains(1))
        {
            userRole = "Admin";
        }
        else if (roleIds.Contains(3))
        {
            userRole = "Student";
        }
        else
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var studentDto = await _unitOfWork.Auth.GetStudentByUserIdAsync(user.UserId, cancellationToken);

        var userDetails = new UserDetailsDto
        {
            UserId = user.UserId,
            Email = user.Email,
            IsActive = user.IsActive,
            Roles = new[] { userRole },
            StudentId = studentDto?.StudentId,
            StudentNumber = studentDto?.StudentNumber,
            FullName = studentDto?.FullName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        var token = _jwtTokenService.GenerateToken(user, userRole, studentDto?.StudentId);
        var expiresAt = DateTime.UtcNow.AddMinutes(120);

        return Ok(new LoginResponseDto(token, "Bearer", expiresAt, userDetails));
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int currentUserId))
        {
            return Unauthorized(new { message = "Invalid token claims." });
        }

        var userDetails = await _unitOfWork.Auth.GetUserDetailsByIdAsync(currentUserId, cancellationToken);
        if (userDetails == null)
        {
            return NotFound(new { message = "User not found." });
        }

        return Ok(userDetails);
    }
}
