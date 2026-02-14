using PostsSocialMedia.Api.Entities.Notification;

namespace PostsSocialMedia.Api.Repositories;

public class NotificationRepository : JsonRepository<Notification>, INotificationRepository
{
    public NotificationRepository() : base("Notifications")
    {
        
    }
}
