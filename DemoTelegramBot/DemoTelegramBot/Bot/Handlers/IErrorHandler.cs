using Telegram.Bot;

namespace DemoTelegramBot.Bot.Handlers;

public interface IErrorHandler
{
    Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct);
}
