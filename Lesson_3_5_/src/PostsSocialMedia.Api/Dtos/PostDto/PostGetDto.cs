using PostsSocialMedia.Api.Dtos.ReactionDto;

namespace PostsSocialMedia.Api.Dtos.PostDto;

public class PostGetDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ReactionStatsDto StatsDto { get; set; }
    public int CommentsCount { get; set; }
}
