using Microsoft.Extensions.Localization;
using StudentCoursePlatform.Application.Resources;
using System.Net;
using System.Text.Json;

namespace StudentCoursePlatform.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IStringLocalizer<SharedResource> localizer,
            IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _localizer = localizer;
        _env = env;
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

            string errorMessage = _env.IsDevelopment()
                ? ex.Message
                : _localizer["ServerError"].Value;

            var response = new
            {
                isSuccess = false,
                errors = new[] { errorMessage }
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}