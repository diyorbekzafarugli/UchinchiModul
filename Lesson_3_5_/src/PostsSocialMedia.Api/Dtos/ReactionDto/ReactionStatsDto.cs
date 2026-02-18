using PostsSocialMedia.Api.Entities.Reaction;

namespace PostsSocialMedia.Api.Dtos.ReactionDto;

public class ReactionStatsDto
{
    public Guid TargetId { get; set; }
    public Dictionary<ReactionType, int> Counts { get; set; } = new();
    public int TotalCount { get; set; }
    public ReactionType? MyReaction { get; set; }
}
