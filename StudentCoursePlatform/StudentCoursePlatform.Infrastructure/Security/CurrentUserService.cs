using Microsoft.AspNetCore.Http;
using StudentCoursePlatform.Application.Interfaces;
using System.Security.Claims;

namespace StudentCoursePlatform.Infrastructure.Security;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContext;

    public CurrentUserService(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
    }
    public Guid UserId
    {
        get
        {
            var context = _httpContext.HttpContext
                ?? throw new UnauthorizedAccessException("HTTP context not found");

            var value = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User id not found in token");

            if (!Guid.TryParse(value, out var userId))
                throw new UnauthorizedAccessException("User id is not valid");

            return userId;
        }
    }

    public string RawToken => _httpContext.HttpContext?
        .Request.Headers.Authorization.ToString().Replace("Bearer ", "") ?? string.Empty;

}
