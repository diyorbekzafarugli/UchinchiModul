using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsSocialMedia.Api.Dtos.UserDto;
using PostsSocialMedia.Api.Services;
using System.Security.Claims;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);

        if (!result.Success) return BadRequest(result.Error);

        return Ok(result.Data);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UserUpdateDto userUpdateDto)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _userService.UpdateAsync(currentUserId, userUpdateDto);
        if (!result.Success) return BadRequest(result.Error);

        return Ok("Profil muvaffaqiyatli yangilandi");
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string userTerm, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _userService.SearchAsync(userTerm, page, pageSize);
        if (!result.Success) return BadRequest(result.Error);

        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var currentUserId = GetUserIdFromToken();

        if (id != currentUserId) return Forbid("Siz faqat o'z profilingizni o'chira olasiz");

        var result = await _userService.DeleteAsync(id);
        if (!result.Success) return BadRequest(result.Error);

        return Ok("Foydalanuvchi o'chirildi");
    }

    [HttpGet("by-user-name")]
    public async Task<IActionResult> GetByUserName(string userName)
    {
        var result = await _userService.GetByUserNameAsync(userName);
        if (!result.Success) return BadRequest(result.Error);

        return Ok(result.Data);
    }

    private Guid GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
    }
}