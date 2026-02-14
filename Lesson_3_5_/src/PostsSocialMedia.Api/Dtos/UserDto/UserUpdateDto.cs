using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Dtos.UserDto;

public class UserUpdateDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public DateTime DateOfBirth { get; set; }
}
