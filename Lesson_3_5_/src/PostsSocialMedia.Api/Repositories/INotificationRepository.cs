using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Repositories;

public interface INotificationRepository
{
    void Add(Notification notification);
    Notification? GetById(Guid id);
    IReadOnlyList<Notification> GetAll();
    bool Update(Notification notificationUpdated);
    bool Delete(Guid id);
}
