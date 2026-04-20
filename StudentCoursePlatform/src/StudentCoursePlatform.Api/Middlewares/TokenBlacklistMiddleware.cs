using Microsoft.EntityFrameworkCore;
using StudentCoursePlatform.Infrastructure.Persistence;

namespace StudentCoursePlatform.Api.Middlewares;

public class TokenBlacklistMiddleware
{
    private readonly RequestDelegate _next;

    public TokenBlacklistMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, AppDbContext dbContext)
    {
        var token = httpContext.Request.Headers.Authorization
            .ToString().Replace("Bearer ", "");
        if (!string.IsNullOrEmpty(token))
        {
            var isBlacklisted = await dbContext.BlacklistedTokens
                .AnyAsync(t => t.Token == token && t.ExpiresAt > DateTime.UtcNow);

            if (isBlacklisted)
            {
                httpContext.Response.StatusCode = 401;
                var response = new { message = "Token is blacklisted" };
                await httpContext.Response.WriteAsJsonAsync(response);
                return;
            }
        }

        await _next(httpContext);
    }
}
