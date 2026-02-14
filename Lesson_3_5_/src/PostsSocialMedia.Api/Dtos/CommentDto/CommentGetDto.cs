namespace PostsSocialMedia.Api.Dtos.CommentDto;

public class CommentGetDto
{
    public Guid Id { get; set; }
    public Guid? ParentCommentId { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
