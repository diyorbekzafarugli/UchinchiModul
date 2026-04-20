using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentCoursePlatform.Application.Common;
using StudentCoursePlatform.Application.DTOs.Users.Requests;
using StudentCoursePlatform.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAsync([FromBody] UserCreateDto dto, 
        CancellationToken ct)
    {
        var result = await _userService.CreateAsync(dto, ct);
        if (!result.IsSuccess) return BadRequest(result.Errors);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParams pagination,
        CancellationToken ct)
    {
        var result = await _userService.GetAllAsync(pagination, ct);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _userService.GetByIdAsync(id, ct);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Errors);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _userService.DeleteAsync(id, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] UserUpdateDto dto,
        CancellationToken ct)
    {
        var result = await _userService.UpdateAsync(dto, ct);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPatch("change-password")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto dto,
        CancellationToken ct)
    {
        var result = await _userService.ChangePasswordAsync(dto, ct);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Errors);
    }
}