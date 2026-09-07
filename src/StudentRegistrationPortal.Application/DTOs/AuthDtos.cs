using System.ComponentModel;

namespace StudentRegistrationPortal.Application.DTOs;

public record LoginRequestDto
{
    [DefaultValue("karim1.bassem@student.edu")]
    public string Email { get; init; } = "karim1.bassem@student.edu";

    [DefaultValue("P@ssw0rd123!")]
    public string Password { get; init; } = "P@ssw0rd123!";
}

public record UserDetailsDto
{
    public int UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
    public int? StudentId { get; init; }
    public string? StudentNumber { get; init; }
    public string? FullName { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record LoginResponseDto(
    string Token,
    string TokenType,
    DateTime ExpiresAt,
    UserDetailsDto User
);
