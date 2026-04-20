using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentCoursePlatform.Application.DTOs.Users.Requests;
using StudentCoursePlatform.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterDto dto,
        CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(dto, ct);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return StatusCode(201, result.Data);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] UserLoginDto dto,
        CancellationToken ct)
    {
        var result = await _authService.LoginAsync(dto, ct);

        if (!result.IsSuccess)
            return Unauthorized(new { message = "Email yoki parol xato", errors = result.Errors });

        return Ok(result.Data);
    }

    [AllowAnonymous] 
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequestDto dto,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(dto.RefreshToken))
            return BadRequest("Refresh token bo'sh bo'lishi mumkin emas.");

        var result = await _authService.RefreshAsync(dto.RefreshToken, ct);

        if (!result.IsSuccess)
            return Unauthorized(result.Errors);

        return Ok(result.Data);
    }
}