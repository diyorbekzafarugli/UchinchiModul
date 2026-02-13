using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DemoTelegramBot.Bot.Handlers;

public sealed class UpdateHandler : IUpdateHandler
{
    private readonly IMessageHandler _message;
    private readonly ICallbackHandler _callback;

    public UpdateHandler(IMessageHandler message, ICallbackHandler callback)
    {
        _message = message;
        _callback = callback;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        if (update.Type == UpdateType.Message && update.Message?.Text is not null)
        {
            await _message.HandleAsync(botClient, update.Message, ct);
            return;
        }

        if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery is not null)
        {
            await _callback.HandleAsync(botClient, update.CallbackQuery, ct);
            return;
        }
    }
}
