namespace PostsSocialMedia.Api.Dtos.CommentDto;

public class CommentAddDto
{
    public Guid? ParentCommentId { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
}
