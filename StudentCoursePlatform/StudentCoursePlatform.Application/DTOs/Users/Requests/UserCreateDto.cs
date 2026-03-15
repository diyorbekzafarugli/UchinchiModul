using StudentCoursePlatform.Domain.Enums;

namespace StudentCoursePlatform.Application.DTOs.Users.Requests;

public class UserCreateDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Student;
}
