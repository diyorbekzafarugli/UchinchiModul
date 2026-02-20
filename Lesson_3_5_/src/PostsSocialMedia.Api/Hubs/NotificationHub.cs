using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PostsSocialMedia.Api.Hubs;

[Authorize]
public class NotificationHub : Hub
{
}
