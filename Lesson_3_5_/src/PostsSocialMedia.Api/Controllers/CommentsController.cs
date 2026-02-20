using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsSocialMedia.Api.Dtos.CommentDto;
using PostsSocialMedia.Api.Services;
using System.Security.Claims;

namespace PostsSocialMedia.Api.Controllers;

[ApiController]
[Route("api/comments")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    public async Task<IActionResult> Add(CommentAddDto commentAddDto)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _commentService.Add(currentUserId, commentAddDto);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _commentService.GetById(currentUserId, id);
        if (result is null) return NotFound();

        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _commentService.GetAll(currentUserId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _commentService.Delete(currentUserId, id);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("get-all-by-post-id")]
    public async Task<IActionResult> GetAlByPostId(Guid id)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _commentService.GetByPostId(currentUserId, id);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("get-user-comments-in-post")]
    public async Task<IActionResult> GetUserCommentsInPost(Guid postId)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _commentService.GetUserCommentsInPost(currentUserId, postId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }
    private Guid GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
    }
}
