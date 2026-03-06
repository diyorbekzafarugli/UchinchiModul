using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudentCoursePlatform.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentCoursePlatform.Infrastructure.Security;

public class JwtHelper
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiryMinutes;
    public JwtHelper(IConfiguration configuration, int expiryMinutes)
    {
        _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
            ?? throw new InvalidOperationException("JWP_SECRET_KEY is missing");
        _issuer = configuration["JwtOptions:Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer missing");
        _audience = configuration["JwtOptions:Audince"]
            ?? throw new InvalidOperationException("JWT Audince missing");
        _expiryMinutes = expiryMinutes;
    }

    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),

                ValidateIssuer = true,
                ValidIssuer = _issuer,

                ValidateAudience = true,
                ValidAudience = _audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public Guid GetUserIdFromToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("Token is null or empty");

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtClaim = tokenHandler.ReadJwtToken(token);

        var userIdClaim = jwtClaim.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
            throw new InvalidOperationException("UserId claim missing in token");

        return Guid.Parse(userIdClaim.Value);
    }
}
