using Microsoft.AspNetCore.Mvc;
using MySocialMedia.Api.Dtos;
using MySocialMedia.Api.Entities;
using MySocialMedia.Api.Services;

namespace MySocialMedia.Api.Controllers;

[Route("api/users")]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{userId:guid}")]
    public ActionResult<Result<UserGetDto>> GetById(
        [FromHeader(Name = "X-UserId")] Guid xUserId,
        [FromHeader(Name = "X-Role")] string xRole,
        Guid userId)
    {
        var token = BuildToken(xUserId, xRole);
        var result = _userService.GetById(token, userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("by-username")]
    public ActionResult<Result<UserGetDto>> GetByUserName(
        [FromHeader(Name = "X-UserId")] Guid xUserId,
        [FromHeader(Name = "X-Role")] string xRole,
        [FromQuery] string userName)
    {
        var token = BuildToken(xUserId, xRole);
        var result = _userService.GetByUserName(token, userName);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet]
    public ActionResult<Result<IReadOnlyList<UserGetDto>>> GetAll(
        [FromHeader(Name = "X-UserId")] Guid xUserId,
        [FromHeader(Name = "X-Role")] string xRole)
    {
        var token = BuildToken(xUserId, xRole);
        var result = _userService.GetAll(token);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("me/profile")]
    public ActionResult<Result<bool>> UpdateProfile(
        [FromHeader(Name = "X-UserId")] Guid xUserId,
        [FromHeader(Name = "X-Role")] string xRole,
        [FromBody] UpdateProfileDto dto)
    {
        var token = BuildToken(xUserId, xRole);
        var result = _userService.UpdateProfile(token, dto.UserName, dto.FullName, dto.DateOfBirth);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{userId:guid}/role")]
    public ActionResult<Result<bool>> UpdateRole(
        [FromHeader(Name = "X-UserId")] Guid xUserId,
        [FromHeader(Name = "X-Role")] string xRole,
        Guid userId,
        [FromBody] UpdateRoleDto dto)
    {
        var token = BuildToken(xUserId, xRole);
        var result = _userService.UpdateRole(token, userId, dto.Role);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("me/password")]
    public ActionResult<Result<bool>> ChangePassword(
        [FromHeader(Name = "X-UserId")] Guid xUserId,
        [FromHeader(Name = "X-Role")] string xRole,
        [FromBody] ChangePasswordDto dto)
    {
        var token = BuildToken(xUserId, xRole);
        var result = _userService.ChangePassword(token, dto.NewPassword);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{userId:guid}/block")]
    public ActionResult<Result<bool>> BlockUser(
        [FromHeader(Name = "X-UserId")] Guid xUserId,
        [FromHeader(Name = "X-Role")] string xRole,
        Guid userId,
        [FromBody] BlockUserDto dto)
    {
        var token = BuildToken(xUserId, xRole);
        var result = _userService.BlockUser(token, userId, dto.Reason, dto.BlockedUntilDays);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{userId:guid}/unblock")]
    public ActionResult<Result<bool>> UnblockUser(
        [FromHeader(Name = "X-UserId")] Guid xUserId,
        [FromHeader(Name = "X-Role")] string xRole,
        Guid userId)
    {
        var token = BuildToken(xUserId, xRole);
        var result = _userService.UnblockUser(token, userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{userId:guid}")]
    public ActionResult<Result<bool>> Delete(
        [FromHeader(Name = "X-UserId")] Guid xUserId,
        [FromHeader(Name = "X-Role")] string xRole,
        Guid userId)
    {
        var token = BuildToken(xUserId, xRole);
        var result = _userService.Delete(token, userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
