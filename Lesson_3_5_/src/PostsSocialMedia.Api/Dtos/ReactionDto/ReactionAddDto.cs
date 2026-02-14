using PostsSocialMedia.Api.Entities.Reaction;

namespace PostsSocialMedia.Api.Dtos.ReactionDto;

public class ReactionAddDto
{
    public Guid UserId { get; set; }
    public Guid TargetId { get; set; }
    public ReactionType Type { get; set; }
}
