using Microsoft.AspNetCore.Mvc;
using MySocialMedia.Api.Dtos;
using MySocialMedia.Api.Entities;
using MySocialMedia.Api.Services;

namespace MySocialMedia.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthorizeService _auth;

    public AuthController(IAuthorizeService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    public ActionResult<Result<Token>> Register([FromBody] UserRegisterDto dto)
    {
        var result = _auth.Create(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
