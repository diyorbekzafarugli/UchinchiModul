using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;

namespace SocialMedia.Api.Entities;

public class Post
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? EditedAt { get; set; }
}
