namespace MySocialMedia.Api.Dtos;

public class PostGetDto
{
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
