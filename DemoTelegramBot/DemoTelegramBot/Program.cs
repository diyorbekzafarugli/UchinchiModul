using System;
using System.Threading;
using System.Threading.Tasks;
using DemoTelegramBot.Bot.Core;
using DemoTelegramBot.Bot.Handlers;
using DemoTelegramBot.Bot.UI;
using DemoTelegramBot.Repositories;
using DemoTelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace DemoTelegramBot;

internal class Program
{
    static async Task Main()
    {
        var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("TELEGRAM_BOT_TOKEN topilmadi. PowerShell: setx TELEGRAM_BOT_TOKEN \"TOKEN\"");
            return;
        }

        // 1) Bot client
        ITelegramBotClient botClient = new TelegramBotClient(token);

        // 2) Repositories
        IUserRepository userRepo = new UserRepository();
        IPostRepository postRepo = new PostRepository();
        IUserService userService = new UserService(userRepo);

        // 3) Context (sessions)
        var ctx = new BotContext();

        // 4) Services
        IAuthService authService = new AuthService(userRepo);
        IPostService postService = new PostService(postRepo, userService);   // <-- sening IPostService implementation
        var adminService = new AdminService(userRepo);          // <-- AdminService class

        // 5) UI
        var ui = new BotUi(ctx, authService, userRepo, postRepo);

        // 6) Handlers
        IMessageHandler messageHandler = new MessageHandler(ctx, authService, postService, adminService, ui);
        ICallbackHandler callbackHandler = new CallbackHandler(ctx, ui, authService, userRepo, postRepo);

        DemoTelegramBot.Bot.Handlers.IUpdateHandler updateHandler =
    new DemoTelegramBot.Bot.Handlers.UpdateHandler(messageHandler, callbackHandler);

        IErrorHandler errorHandler = new ErrorHandler();

        // 7) Polling
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>(),
            DropPendingUpdates = true
        };

        botClient.StartReceiving(
            updateHandler: updateHandler.HandleUpdateAsync,
            errorHandler: errorHandler.HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await botClient.GetMe();
        Console.WriteLine($"✅ @{me.Username} ishlayapti. To'xtatish: Ctrl+C");

        await Task.Delay(-1, cts.Token);
    }
}
