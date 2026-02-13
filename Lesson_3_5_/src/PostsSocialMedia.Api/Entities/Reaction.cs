
namespace PostsSocialMedia.Api.Entities;

public class Reaction : IEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TargetId { get; set; }
    public ReactionType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}
