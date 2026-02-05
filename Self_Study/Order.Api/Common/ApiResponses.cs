using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Order.Api.Common;

public static class ApiResponses
{
    public static ApiResponse<T> Ok<T>(T data, string message, HttpContext? ctx = null)
        => new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = null,
            TraceId = ctx?.TraceIdentifier
        };

    public static ApiResponse<T> Ok<T>(string message, HttpContext? ctx = null)
        => new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = default,
            Errors = null,
            TraceId = ctx?.TraceIdentifier
        };
    public static ApiResponse<T> Fail<T>(string message, IEnumerable<string>? errors = null, HttpContext? ctx = null)
        => new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = default,
            Errors = errors?.ToList(),
            TraceId = ctx?.TraceIdentifier
        };
}
