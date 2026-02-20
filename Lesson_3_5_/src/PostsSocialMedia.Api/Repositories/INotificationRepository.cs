using PostsSocialMedia.Api.Entities.Notification;

namespace PostsSocialMedia.Api.Repositories;

public interface INotificationRepository
{
    Task Add(Notification notification);
    Task<Notification?> GetById(Guid id);
    Task<List<Notification>> GetByUserId(Guid userId, int limit);
    Task<int> GetUnreadCount(Guid userId);
    Task Update(Notification notification);
    Task <IReadOnlyList<Notification>> GetAll();
    Task Delete(Guid id);
    Task<List<Notification>> GetByUserIdAndType(Guid userId, TypeNotification type);
}
