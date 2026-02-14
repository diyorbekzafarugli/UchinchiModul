using PostsSocialMedia.Api.Entities.Reaction;

namespace PostsSocialMedia.Api.Dtos.ReactionDto;

public class ReactionGetDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TargetId { get; set; }
    public ReactionType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}
