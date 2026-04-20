using System.Security.Principal;

namespace StudentCoursePlatform.Application.DTOs.Users.Responses;

public class AuthResponseDto
{
    public UserResponseDto User { get; set; } = default!;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpireAt { get; set; }

}
