using PostsSocialMedia.Api.Dtos.NotificationDto;
using PostsSocialMedia.Api.Entities;
using PostsSocialMedia.Api.Entities.Notification;

namespace PostsSocialMedia.Api.Services;

public interface INotificationService
{
    Task<Result<Guid>> Send(Guid currentUserId, NotificationAddDto dto);

    Task<Result<List<NotificationGetDto>>> GetFeed(Guid currentUserId, int limit = 20);

    Task<Result<List<NotificationGetDto>>> GetByType(Guid currentUserId, TypeNotification type);

    Task<Result<bool>> MarkAsRead(Guid currentUserId, Guid id);

    Task<Result<bool>> Delete(Guid currentUserId, Guid id);

    Task<Result<int>> GetNotificationCount(Guid currentUserId);
}