using StudentCoursePlatform.Domain.Enums;
using System.Text.Json.Serialization;

namespace StudentCoursePlatform.Application.DTOs.Users.Responses;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole Role { get; set; }

    public DateTime CreatedAt { get; set; }
}
