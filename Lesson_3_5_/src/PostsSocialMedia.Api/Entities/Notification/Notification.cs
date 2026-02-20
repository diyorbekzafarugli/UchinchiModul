namespace PostsSocialMedia.Api.Entities.Notification;

public class Notification : IEntity
{
    public Guid Id { get; set; }
    public Guid ToUserId { get; set; }
    public Guid FromUserId { get; set; }
    public TypeNotification Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
