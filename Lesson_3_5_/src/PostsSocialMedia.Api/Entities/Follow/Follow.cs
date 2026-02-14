namespace PostsSocialMedia.Api.Entities.Follow;

public class Follow : IEntity
{
    public Guid Id { get; set; }
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
    public DateTime FollowedAt { get; set; }
}
