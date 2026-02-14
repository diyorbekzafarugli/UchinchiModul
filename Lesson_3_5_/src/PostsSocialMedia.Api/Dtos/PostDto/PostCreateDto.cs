namespace PostsSocialMedia.Api.Dtos.PostDto;

public class PostCreateDto
{
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}
