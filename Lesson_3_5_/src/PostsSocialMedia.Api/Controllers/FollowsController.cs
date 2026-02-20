using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsSocialMedia.Api.Dtos.FollowDto;
using PostsSocialMedia.Api.Services;
using System.Security.Claims;

namespace PostsSocialMedia.Api.Controllers;

[ApiController]
[Route("api/follows")]
[Authorize]
public class FollowsController : ControllerBase
{
    private readonly IFollowService _followService;
    public FollowsController(IFollowService followService)
    {
        _followService = followService;
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(FollowAddDto followAddDto)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _followService.Add(currentUserId, followAddDto);
        if (!result.Success) return BadRequest(result.Error);
        return Ok(result.Data);
    }

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _followService.GetById(currentUserId, id);
        if (!result.Success) return BadRequest(result.Error);
        return Ok(result.Data);
    }

    [HttpGet("get-by-user-name")]
    public async Task<IActionResult> GetByUserNameAsync(string searchTerm, int page, int pageSize)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest("Foydalanuvchi ismini bo'sh kitirdingiz");
        if ((page <= 0 && page >= 100) || (pageSize <= 0 && pageSize > 20)) return BadRequest("Sahifani kitishda xatolik");

        var result = await _followService.GetUsersByName(currentUserId, searchTerm, page, pageSize);
        if (!result.Success) return BadRequest(result.Error);
        return Ok(result.Data);
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllAsync()
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _followService.GetAll(currentUserId);
        if (!result.Success) return BadRequest(result.Error);
        return Ok(result.Data);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _followService.Delete(currentUserId, id);
        if (!result.Success) return BadRequest(result.Error);
        return Ok(result.Data);
    }
    private Guid GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
    }
}
