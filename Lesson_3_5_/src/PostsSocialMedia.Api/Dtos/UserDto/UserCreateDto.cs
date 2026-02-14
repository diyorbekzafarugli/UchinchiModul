using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Dtos.UserDto;

public class UserCreateDto
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public DateTime DateOfBirth { get; set; }
}
