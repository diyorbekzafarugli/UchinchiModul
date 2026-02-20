using Microsoft.AspNetCore.Mvc;
using PostsSocialMedia.Api.Dtos.UserDto;
using PostsSocialMedia.Api.Services;

namespace PostsSocialMedia.Api.Controllers;

[ApiController]
[Route("api/(authorize)")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserCreateDto userCreateDto)
    {
        var result = await _authService.RegisterAsync(userCreateDto);
        if (!result.Success) return BadRequest(result.Error);

        return Ok(result.Data);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string userName, string password)
    {
        var result = await _authService.LoginUserAsync(userName, password);
        if (!result.Success) return Unauthorized(result.Error);

        return Ok(new { Token = result.Data });
    }
}
