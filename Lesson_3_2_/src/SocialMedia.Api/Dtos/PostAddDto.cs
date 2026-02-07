namespace SocialMedia.Api.Dtos;

public class PostAddDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
