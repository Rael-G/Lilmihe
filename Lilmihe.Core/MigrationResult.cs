namespace Lilmihe;

public class MigrationResult
{
    public bool Success { get; set; } = false;

    public string? FailedCommand { get; set; } = string.Empty;

    public string? FailedFile { get; set; } = string.Empty;

    public string[] SuccessFiles { get; set; } = [];

    public string Message { get; set; } = string.Empty;

    public Exception? Error { get; set; }
}
