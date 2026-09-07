namespace StudentRegistrationPortal.Application.DTOs;

public record LookupItemDto(int Id, string Name, string? Extra = null);

public record LookupsResponseDto
{
    public IReadOnlyList<LookupItemDto> Departments { get; init; } = Array.Empty<LookupItemDto>();
    public IReadOnlyList<LookupItemDto> Semesters { get; init; } = Array.Empty<LookupItemDto>();
    public IReadOnlyList<LookupItemDto> Instructors { get; init; } = Array.Empty<LookupItemDto>();
    public IReadOnlyList<LookupItemDto> Rooms { get; init; } = Array.Empty<LookupItemDto>();
    public IReadOnlyList<LookupItemDto> CourseStatuses { get; init; } = Array.Empty<LookupItemDto>();
}
