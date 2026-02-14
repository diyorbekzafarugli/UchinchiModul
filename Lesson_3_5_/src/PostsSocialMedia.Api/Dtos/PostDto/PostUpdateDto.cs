namespace PostsSocialMedia.Api.Dtos.PostDto;

public class PostUpdateDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}
