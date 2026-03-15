using Microsoft.Extensions.Localization;
using System.Net;
using System.Text.Json;

namespace StudentCoursePlatform.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IStringLocalizer<GlobalExceptionMiddleware> _localizer;
    public GlobalExceptionMiddleware(RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IStringLocalizer<GlobalExceptionMiddleware> localizer)
    {
        _next = next;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                statusCode = (int)HttpStatusCode.InternalServerError,
                message = _localizer["ServerError"]
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
