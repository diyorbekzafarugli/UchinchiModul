using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace DemoTelegramBot.Bot.Handlers;

public sealed class ErrorHandler : IErrorHandler
{
    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
    {
        var msg = exception switch
        {
            ApiRequestException api => $"Telegram API Error:\n[{api.ErrorCode}] {api.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(msg);
        return Task.CompletedTask;
    }
}
