using Microsoft.AspNetCore.Mvc;
using MySocialMedia.Api.Dtos;
using MySocialMedia.Api.Entities;
using MySocialMedia.Api.Services;

namespace MySocialMedia.Api.Controllers;

[Route("api/posts")]
public class PostsController : BaseApiController
{
    private readonly IPostService _postService;
    public PostsController(IPostService postService) => _postService = postService;

    [HttpPost]
    public ActionResult<Result<Guid>> Add(
        [FromHeader(Name = "X-UserId")] Guid xUserId,
        [FromHeader(Name = "X-Role")] string xRole,
        [FromBody] PostAddDto dto)
    {
        var token = BuildToken(xUserId, xRole);
        var result = _postService.Add(token, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("my")]
    public ActionResult<Result<IReadOnlyList<PostGetDto>>> GetMyPosts(
        [FromHeader(Name = "X-UserId")] Guid xUserId,
        [FromHeader(Name = "X-Role")] string xRole)
    {
        var token = BuildToken(xUserId, xRole);
        var result = _postService.GetMyPosts(token);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{postId:guid}")]
    public ActionResult<Result<PostGetDto>> GetById(
        [FromHeader(Name = "X-UserId")] Guid xUserId,
        [FromHeader(Name = "X-Role")] string xRole,
        Guid postId)
    {
        var token = BuildToken(xUserId, xRole);
        var result = _postService.GetById(token, postId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{postId:guid}")]
    public ActionResult<Result<bool>> Update(
        [FromHeader(Name = "X-UserId")] Guid xUserId,
        [FromHeader(Name = "X-Role")] string xRole,
        Guid postId,
        [FromBody] UpdatePostDto dto)
    {
        var token = BuildToken(xUserId, xRole);
        var result = _postService.Update(token, postId, dto.Title, dto.Content);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{postId:guid}")]
    public ActionResult<Result<bool>> Delete(
        [FromHeader(Name = "X-UserId")] Guid xUserId,
        [FromHeader(Name = "X-Role")] string xRole,
        Guid postId)
    {
        var token = BuildToken(xUserId, xRole);
        var result = _postService.Delete(token, postId);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
