using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsSocialMedia.Api.Dtos.NotificationDto;
using PostsSocialMedia.Api.Entities.Notification;
using PostsSocialMedia.Api.Services;
using System.Security.Claims;

namespace PostsSocialMedia.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost("send-notification")]
    public async Task<IActionResult> SendAsync(NotificationAddDto notificationAddDto)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _notificationService.Send(currentUserId, notificationAddDto);
        if (!result.Success) return BadRequest(result.Error);
        return Ok(result.Data);
    }

    [HttpGet("get-notifications")]
    public async Task<IActionResult> GetAll(int limit)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _notificationService.GetFeed(currentUserId, limit);
        if (!result.Success) return BadRequest(result.Error);
        return Ok(result.Data);
    }

    [HttpGet("get-by-type")]
    public async Task<IActionResult> GetByType(TypeNotification typeNotification)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _notificationService.GetByType(currentUserId, typeNotification);
        if (!result.Success) return BadRequest(result.Error);
        return Ok(result.Data);
    }

    [HttpPatch("mark-as-read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _notificationService.MarkAsRead(currentUserId, id);
        if (!result.Success) return BadRequest(result.Error);
        return Ok(result.Data);
    }

    [HttpGet("get-notification-count")]
    public async Task<IActionResult> GetNotificationCount()
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _notificationService.GetNotificationCount(currentUserId);
        if (!result.Success) return BadRequest(result.Error);
        return Ok(result.Data);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var currentUserId = GetUserIdFromToken();
        if (currentUserId == Guid.Empty) return Unauthorized();

        var result = await _notificationService.Delete(currentUserId, id);
        if (!result.Success) return BadRequest(result.Error);
        return Ok(result.Data);
    }
    private Guid GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
    }
}
