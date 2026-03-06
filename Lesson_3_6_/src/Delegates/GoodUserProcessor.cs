namespace Delegates;

public class GoodUserProcessor
{
    public void ProcessUser(List<User> users, Action<User> action)
    {
        foreach (var user in users)
        {
            action(user);
        }
    }
}
