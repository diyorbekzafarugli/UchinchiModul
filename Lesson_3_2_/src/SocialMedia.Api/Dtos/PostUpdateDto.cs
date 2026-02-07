namespace SocialMedia.Api.Dtos;

public class PostUpdateDto
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
