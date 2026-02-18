using PostsSocialMedia.Api.Entities.Reaction;

namespace PostsSocialMedia.Api.Dtos.ReactionDto;

public class ReactionAddDto
{
    public Guid TargetId { get; set; }
    public ReactionType Type { get; set; }
}
