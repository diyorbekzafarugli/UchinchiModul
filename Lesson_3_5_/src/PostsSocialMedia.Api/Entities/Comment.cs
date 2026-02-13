namespace PostsSocialMedia.Api.Entities;

public class Comment : IEntity
{
    public Guid Id { get; set; }
    public Guid? ParentCommentId { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
