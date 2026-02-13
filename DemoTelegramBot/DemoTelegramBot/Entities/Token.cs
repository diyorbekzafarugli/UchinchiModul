using System.Reflection.Metadata.Ecma335;

namespace DemoTelegramBot.Entities;

public class Token
{
    public Guid UserId { get; set; }
    public UserRole Role { get; set; }

}
