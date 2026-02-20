using PostsSocialMedia.Api.Entities.Notification;

namespace PostsSocialMedia.Api.Dtos.NotificationDto;

public class NotificationGetDto
{
    public Guid Id { get; set; }
    public TypeNotification Type { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public Guid FromUserId { get; set; }
    public string FromUserName { get; set; }
    public string FromUserProfilePhoto { get; set; } // Profil rasm URL
    public Guid? RelatedEntityId { get; set; } // PostId yoki CommentId
}
