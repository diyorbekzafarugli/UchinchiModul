namespace Order.Api.Common;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = "";
    public T? Data { get; init; }
    public List<string>? Errors { get; init; }

    public string? TraceId { get; init; }
    public DateTimeOffset Time { get; init; } = DateTimeOffset.UtcNow;
}
