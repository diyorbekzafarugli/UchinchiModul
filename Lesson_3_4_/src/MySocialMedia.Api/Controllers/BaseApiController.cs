using Microsoft.AspNetCore.Mvc;
using MySocialMedia.Api.Entities;

namespace MySocialMedia.Api.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected Token BuildToken(Guid xUserId, string xRole)
    {
        if (!Enum.TryParse<UserRole>(xRole, ignoreCase: true, out var role))
            role = UserRole.User;

        return new Token { UserId = xUserId, Role = role };
    }
}
