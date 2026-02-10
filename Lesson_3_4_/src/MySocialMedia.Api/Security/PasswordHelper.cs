using Microsoft.AspNetCore.Identity;
using MySocialMedia.Api.Entities;

namespace MySocialMedia.Api.Security;

public static class PasswordHelper
{
    private static readonly PasswordHasher<User> _hasher = new();

    public static string Hash(User user, string password)
        => _hasher.HashPassword(user, password);

    public static bool Verify(User user, string hashedPassword, string providedPassword)
        => _hasher.VerifyHashedPassword(user, hashedPassword, providedPassword)
           != PasswordVerificationResult.Failed;
}
