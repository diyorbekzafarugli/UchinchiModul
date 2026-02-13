using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DemoTelegramBot.Bot.Handlers;

public interface IUpdateHandler
{
    Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct);
}
