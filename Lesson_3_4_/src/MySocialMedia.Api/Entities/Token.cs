using System.Reflection.Metadata.Ecma335;

namespace MySocialMedia.Api.Entities;

public class Token
{
    public Guid UserId { get; set; }
    public UserRole Role { get; set; }

}
