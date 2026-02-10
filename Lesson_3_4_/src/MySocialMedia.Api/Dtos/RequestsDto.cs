using MySocialMedia.Api.Entities;

namespace MySocialMedia.Api.Dtos;

public record UpdateProfileDto(string UserName, string FullName, DateTime DateOfBirth);
public record ChangePasswordDto(string NewPassword);
public record UpdateRoleDto(UserRole Role);
public record BlockUserDto(string? Reason, int BlockedUntilDays);
public record UpdatePostDto(string Title, string Content);

