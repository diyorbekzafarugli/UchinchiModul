using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsSocialMedia.Api.Dtos.ReactionDto;
using PostsSocialMedia.Api.Services;
using System.Security.Claims;

namespace PostsSocialMedia.Api.Controllers;

[ApiController]
[Route("api/reactions")]
[Authorize]
public class ReactionsController : ControllerBase
{
    private readonly IReactionService _reactionService;
    public ReactionsController(IReactionService reactionService)
    {
        _reactionService = reactionService;
    }

    [HttpPost("add-rection")]
    public async Task<IActionResult> Add(ReactionAddDto reactionAddDto)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _reactionService.Add(currentUserId, reactionAddDto);
        if (!result.Success) return BadRequest(result.Error);
        return Ok(result.Data);
    }

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _reactionService.GetById(currentUserId, id);
        if (!result.Success) return BadRequest(result.Error);
        return Ok(result.Data);
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _reactionService.GetAll(currentUserId);
        if (!result.Success) return BadRequest(result.Error);
        return Ok(result.Data);
    }

    [HttpGet("get-stats-by-target-id")]
    public async Task<IActionResult> GetStatsByTargetId(Guid targetId)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _reactionService.GetStatsByTargetId(currentUserId, targetId);
        if (!result.Success) return BadRequest(result.Error);
        return Ok(result.Data);
    }
    private Guid GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
    }
}
