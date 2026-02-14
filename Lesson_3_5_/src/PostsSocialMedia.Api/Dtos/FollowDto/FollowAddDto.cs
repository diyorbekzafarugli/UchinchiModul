namespace PostsSocialMedia.Api.Dtos.FollowDto;

public class FollowAddDto
{
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
}
