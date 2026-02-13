namespace DemoTelegramBot.Dtos;

public class UserRegisterDto
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
}
