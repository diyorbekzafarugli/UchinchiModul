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
    public IActionResult GetById(Guid id)
    {
        var result = _userService.GetById(id);

        if (!result.Success) return BadRequest(result.Error);

        return Ok(result.Data);
    }

    [HttpPut("update")]
    public IActionResult Update(UserUpdateDto userUpdateDto)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = _userService.Update(currentUserId, userUpdateDto);
        if (!result.Success) return BadRequest(result.Error);

        return Ok("Profil muvaffaqiyatli yangilandi");
    }

    [HttpGet("search")]
    public IActionResult Search([FromQuery] string userTerm, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = _userService.Search(userTerm, page, pageSize);
        if (!result.Success) return BadRequest(result.Error);

        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var currentUserId = GetUserIdFromToken();

        if (id != currentUserId) return Forbid("Siz faqat o'z profilingizni o'chira olasiz");

        var result = _userService.Delete(id);
        if (!result.Success) return BadRequest(result.Error);

        return Ok("Foydalanuvchi o'chirildi");
    }

    [HttpGet("by-user-name")]
    public IActionResult GetByUserName(string userName)
    {
        var result = _userService.GetByUserName(userName);
        if (!result.Success) return BadRequest(result.Error);

        return Ok(result.Data);
    }

    private Guid GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
    }
}