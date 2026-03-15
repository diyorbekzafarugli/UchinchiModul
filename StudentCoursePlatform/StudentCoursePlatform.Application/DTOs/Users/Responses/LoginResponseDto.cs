namespace StudentCoursePlatform.Application.DTOs.Users.Responses;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;

    public DateTime AccessTokenExpireAt { get; set; }

    public UserResponseDto User { get; set; } = default!;
}