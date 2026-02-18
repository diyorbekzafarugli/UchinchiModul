using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsSocialMedia.Api.Dtos.PostDto;
using PostsSocialMedia.Api.Services;
using System.Security.Claims;

namespace PostsSocialMedia.Api.Controllers;

[ApiController]
[Route("api/posts")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpPost]
    public IActionResult Create(PostCreateDto postCreateDto)
    {
        var result = _postService.Create(postCreateDto);

        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("get-by-id")]
    public IActionResult GetById(Guid id)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = _postService.GetById(id, currentUserId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("search")]
    public IActionResult Search(string searchTerm, int page = 1, int pageSize = 10)
    {
        var userId = GetUserIdFromToken();
        if (userId == Guid.Empty) return Unauthorized();

        var result = _postService.Search(searchTerm, page, pageSize, userId);
        if (!result.Success) return BadRequest(result.Error);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpPut("update")]
    public IActionResult Update(PostUpdateDto postUpdateDto)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = _postService.Update(currentUserId, postUpdateDto);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("by-user-name")]
    public IActionResult GetBeyUserName(string userName)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = _postService.GetByUserName(userName, currentUserId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpDelete]
    public IActionResult Delete(Guid id)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = _postService.Delete(id, currentUserId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }
    private Guid GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
    }
}
