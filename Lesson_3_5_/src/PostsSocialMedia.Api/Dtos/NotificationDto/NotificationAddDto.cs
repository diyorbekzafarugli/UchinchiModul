using PostsSocialMedia.Api.Entities.Notification;

namespace PostsSocialMedia.Api.Dtos.NotificationDto;

public class NotificationAddDto
{
    public Guid ToUserId { get; set; }
    public TypeNotification Type { get; set; }
    public string Message { get; set; } = string.Empty;
}
