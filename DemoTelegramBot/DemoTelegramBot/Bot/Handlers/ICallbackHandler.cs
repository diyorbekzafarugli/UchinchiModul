using Telegram.Bot;
using Telegram.Bot.Types;

namespace DemoTelegramBot.Bot.Handlers;

public interface ICallbackHandler
{
    Task HandleAsync(ITelegramBotClient botClient, CallbackQuery cb, CancellationToken ct);
}
