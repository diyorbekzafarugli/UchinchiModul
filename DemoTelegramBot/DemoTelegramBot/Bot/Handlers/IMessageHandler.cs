using Telegram.Bot;
using Telegram.Bot.Types;

namespace DemoTelegramBot.Bot.Handlers;

public interface IMessageHandler
{
    Task HandleAsync(ITelegramBotClient botClient, Message msg, CancellationToken ct);
}
