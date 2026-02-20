using MediatR;
using PostsSocialMedia.Api.Dtos.NotificationDto;
using PostsSocialMedia.Api.Entities.Notification;
using PostsSocialMedia.Api.Events;
using PostsSocialMedia.Api.Services;

namespace PostsSocialMedia.Api.Handlers;

public class CommentNotificationHandler : INotificationHandler<CommentAddedEvent>
{
    private readonly INotificationService _notificationService;
    public CommentNotificationHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    public async Task Handle(CommentAddedEvent notification, CancellationToken cancellationToken)
    {
        var action = notification.IsReply ? "javob berdi" : "izoh qoldirdi";

        await _notificationService.Send(notification.RecipientId, new NotificationAddDto
        {
            ToUserId = notification.RecipientId,
            Message = $"{notification.CommenterName} sizga {action} : {notification.Message}",
            Type = TypeNotification.Comment
        });
    }
}
