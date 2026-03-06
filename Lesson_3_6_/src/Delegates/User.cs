namespace Delegates;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public bool IsActive { get; set; } = true;


    public void SendWelcomeEmails(List<User> users)
    {
        foreach (var user in users)
        {
            Console.WriteLine($"Simulating sending welcome email to {user.Email}...");
        }
    }

    public void DeactivateUser(List<User> users)
    {
        foreach (var user in users)
        {
            user.IsActive = false;
        }
    }

    public void PrintUserInfo(List<User> users)
    {
        foreach (var user in users)
        {
            Console.WriteLine($"ID : {user.Id}\tName : {user.Name}\tEmail : {user.Email}\tAge : {user.Age}");
        }
    }
}
