using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using StudentCoursePlatform.Application.Common;
using StudentCoursePlatform.Application.DTOs.Users.Requests;
using StudentCoursePlatform.Application.Interfaces;

namespace StudentCoursePlatform.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IStringLocalizer<UsersController> _localizer;

    public UsersController(IUserService userService,
                           IStringLocalizer<UsersController> localizer)
    {
        _userService = userService;
        _localizer = localizer;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserCreateDto userCreateDto,
        CancellationToken cancellationToken)
    {
        var result = await _userService.CreateAsync(userCreateDto, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Data }, result.Data);
    }

    [HttpGet]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationParams(page, pageSize);
        var result = await _userService.GetAllAsync(pagination, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }

    [HttpDelete("user")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result.Errors);

        return Ok(result.Data);
    }

    [HttpPatch("change-password")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto passwordDto,
        CancellationToken cancellationToken)
    {
        var result = await _userService.ChangePasswordAsync(passwordDto, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }

    [HttpDelete("account")]
    public async Task<IActionResult> DeleteAccountAsync(CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteAccountAsync(cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] UserUpdateDto updateDto,
        CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateAsync(updateDto, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }
}