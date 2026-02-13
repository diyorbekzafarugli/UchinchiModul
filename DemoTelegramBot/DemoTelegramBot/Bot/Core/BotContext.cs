using System.Collections.Concurrent;

namespace DemoTelegramBot.Bot.Core;

public sealed class BotContext
{
    public ConcurrentDictionary<long, Session> Sessions { get; } = new();

    public Session GetSession(long tgUserId)
        => Sessions.GetOrAdd(tgUserId, _ => new Session());
}
