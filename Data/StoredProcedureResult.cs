namespace StudentRegistrationPortal.Api.Data;

/// <summary>
/// Encapsulates the output state from Stored Procedures implementing the ProcessingStatus pattern.
/// </summary>
public record StoredProcedureResult
{
    public bool IsSuccess => ProcessingStatus == 1;
    public int ProcessingStatus { get; init; }
    public string ProcessingMessage { get; init; } = string.Empty;
    public int? AffectedId { get; init; }

    public static StoredProcedureResult Success(string message = "Operation completed successfully.", int? affectedId = null) =>
        new() { ProcessingStatus = 1, ProcessingMessage = message, AffectedId = affectedId };

    public static StoredProcedureResult Failure(string message, int status = 0) =>
        new() { ProcessingStatus = status, ProcessingMessage = message, AffectedId = null };
}
