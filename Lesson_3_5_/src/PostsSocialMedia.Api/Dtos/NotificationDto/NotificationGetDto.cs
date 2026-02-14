using PostsSocialMedia.Api.Entities.Notification;

namespace PostsSocialMedia.Api.Dtos.NotificationDto;

public class NotificationGetDto
{
    public Guid Id { get; set; }
    public Guid FromUserId { get; set; }
    public TypeNotification Type { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}
