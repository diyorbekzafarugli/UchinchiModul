using System.Security.Principal;

namespace PostsSocialMedia.Api.Dtos.FollowDto;

public class FollowGetDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public bool IsFollowedByMe { get; set; }
    public DateTime FollowedAt { get; set; }
}
    