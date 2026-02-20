using PostsSocialMedia.Api.Entities.Notification;

namespace PostsSocialMedia.Api.Repositories;

public class NotificationRepository : JsonRepository<Notification>, INotificationRepository
{
    public NotificationRepository() : base("Notifications") { }

    public async Task<List<Notification>> GetByUserId(Guid userId, int limit)
    {
        var allNotifications = await GetAll();
        return allNotifications.Where(n => n.ToUserId == userId)
                               .OrderByDescending(n => n.CreatedAt)
                               .Take(limit)
                               .ToList();
    }

    public async Task<List<Notification>> GetByUserIdAndType(Guid userId, TypeNotification type)
    {
        var allNotifications = await GetAll();
        return allNotifications.Where(n => n.ToUserId == userId && n.Type == type)
                               .OrderByDescending(n => n.CreatedAt)
                               .ToList();
    }

    public async Task<int> GetUnreadCount(Guid userId)
    {
        var allNotifications = await GetAll();
        return allNotifications.Count(n => n.ToUserId == userId && n.IsRead == false);
    }
}
