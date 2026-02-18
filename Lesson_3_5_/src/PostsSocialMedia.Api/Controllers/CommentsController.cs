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
    public IActionResult Add(CommentAddDto commentAddDto)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = _commentService.Add(currentUserId, commentAddDto);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("get-by-id")]
    public IActionResult GetById(Guid id)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = _commentService.GetById(currentUserId, id);
        if (result is null) return NotFound();

        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("get-all")]
    public IActionResult GetAll()
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = _commentService.GetAll(currentUserId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpDelete]
    public IActionResult Delete(Guid id)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = _commentService.Delete(currentUserId, id);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("get-all-by-post-id")]
    public IActionResult GetAlByPostId(Guid id)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = _commentService.GetByPostId(currentUserId, id);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("get-user-comments-in-post")]
    public IActionResult GetUserCommentsInPost(Guid postId)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = _commentService.GetUserCommentsInPost(currentUserId, postId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }
    private Guid GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
    }
}
