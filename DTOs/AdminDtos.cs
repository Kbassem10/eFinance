using System.ComponentModel;

namespace StudentRegistrationPortal.Api.DTOs;

public record AdminDetailsDto
{
    public int UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record AdminLoginResponseDto(
    string Token,
    string TokenType,
    DateTime ExpiresAt,
    AdminDetailsDto Admin
);

