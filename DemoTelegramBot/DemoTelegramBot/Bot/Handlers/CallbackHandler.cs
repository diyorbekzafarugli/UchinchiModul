using System;
using System.Threading;
using System.Threading.Tasks;
using DemoTelegramBot.Bot.Core;
using DemoTelegramBot.Bot.UI;
using DemoTelegramBot.Repositories;
using DemoTelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DemoTelegramBot.Bot.Handlers;

public sealed class CallbackHandler : ICallbackHandler
{
    private readonly BotContext _ctx;
    private readonly BotUi _ui;
    private readonly IAuthService _auth;
    private readonly IUserRepository _userRepo;
    private readonly IPostRepository _postRepo;

    public CallbackHandler(
        BotContext ctx,
        BotUi ui,
        IAuthService auth,
        IUserRepository userRepo,
        IPostRepository postRepo)
    {
        _ctx = ctx;
        _ui = ui;
        _auth = auth;
        _userRepo = userRepo;
        _postRepo = postRepo;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery cb, CancellationToken ct)
    {
        // Inline callback bo'lsa Message null bo'lishi mumkin
        if (cb.Message is null)
        {
            await botClient.AnswerCallbackQuery(cb.Id, cancellationToken: ct);
            return;
        }

        var tgUserId = cb.From.Id;
        var chatId = cb.Message.Chat.Id;
        var messageId = cb.Message.MessageId;
        var data = cb.Data ?? "";

        var s = _ctx.GetSession(tgUserId);

        // "loading..." yo'qolsin
        await botClient.AnswerCallbackQuery(cb.Id, cancellationToken: ct);

        // ------------------- ASOSIY BUTTONLAR -------------------
        switch (data)
        {
            case "nav:home":
                s.Step = BotStep.None;
                await _ui.SendHome(botClient, chatId, messageId, s, ct);
                return;

            case "auth:register":
                s.Step = BotStep.Register_UserName;
                s.TempUserName = null;
                await _ui.EditOrSend(botClient, chatId, messageId, "🆕 Ro‘yxatdan o‘tish.\nUsername yuboring:", null, ct);
                return;

            case "auth:login":
                s.Step = BotStep.Login_UserName;
                s.TempUserName = null;
                await _ui.EditOrSend(botClient, chatId, messageId, "🔐 Login.\nUsername yuboring:", null, ct);
                return;

            case "auth:logout":
                s.Token = null;
                s.Step = BotStep.None;
                await _ui.EditOrSend(botClient, chatId, messageId, "👋 Logout bo‘ldi.", null, ct);
                await _ui.SendHome(botClient, chatId, null, s, ct);
                return;

            case "post:add":
                if (!IsLoggedIn(s))
                {
                    await _ui.EditOrSend(botClient, chatId, messageId, "❌ Avval login qiling.", null, ct);
                    return;
                }

                s.Step = BotStep.AddPost_Title;
                s.TempTitle = null;
                await _ui.EditOrSend(botClient, chatId, messageId, "📝 Post sarlavhasini (Title) yuboring:", null, ct);
                return;

            case "post:my":
                if (!IsLoggedIn(s))
                {
                    await _ui.EditOrSend(botClient, chatId, messageId, "❌ Avval login qiling.", null, ct);
                    return;
                }

                await _ui.ShowMyPosts(botClient, chatId, messageId, s, ct);
                return;

            case "admin:users":
                if (!IsAdmin(s))
                {
                    await _ui.EditOrSend(botClient, chatId, messageId, "❌ Siz admin emassiz.", null, ct);
                    return;
                }

                await _ui.ShowUsers(botClient, chatId, messageId, s, ct);
                return;

            case "admin:posts_all":
                if (!IsAdmin(s))
                {
                    await _ui.EditOrSend(botClient, chatId, messageId, "❌ Siz admin emassiz.", null, ct);
                    return;
                }

                await _ui.ShowAllPosts(botClient, chatId, messageId, s, ct);
                return;
        }

        // ------------------- DYNAMIC DATA: u:* va post:* -------------------

        // u:open:<userId>
        if (data.StartsWith("u:open:") && Guid.TryParse(data["u:open:".Length..], out var openUserId))
        {
            if (!IsAdmin(s))
            {
                await _ui.EditOrSend(botClient, chatId, messageId, "❌ Siz admin emassiz.", null, ct);
                return;
            }

            // BotUi ichida shu method bo'lishi kerak:
            // ShowUserPanel(ITelegramBotClient botClient, long chatId, int? messageId, Session s, Guid userId, CancellationToken ct)
            await _ui.ShowUserPanel(botClient, chatId, messageId, s, openUserId, ct);
            return;
        }

        // u:posts:<userId>
        if (data.StartsWith("u:posts:") && Guid.TryParse(data["u:posts:".Length..], out var postsUserId))
        {
            if (!IsAdmin(s))
            {
                await _ui.EditOrSend(botClient, chatId, messageId, "❌ Siz admin emassiz.", null, ct);
                return;
            }

            // BotUi ichida shu method bo'lishi kerak:
            // ShowUserPosts(ITelegramBotClient botClient, long chatId, int? messageId, Session s, Guid userId, CancellationToken ct)
            await _ui.ShowUserPosts(botClient, chatId, messageId, s, postsUserId, ct);
            return;
        }

        // u:block7:<userId>
        if (data.StartsWith("u:block7:") && Guid.TryParse(data["u:block7:".Length..], out var b7))
        {
            // BotUi ichida shu method bo'lishi kerak:
            // DoBlockPreset(ITelegramBotClient botClient, long chatId, int messageId, Session s, Guid userId, int days, CancellationToken ct)
            await _ui.DoBlockPreset(botClient, chatId, messageId, s, b7, days: 7, ct);
            return;
        }

        // u:block30:<userId>
        if (data.StartsWith("u:block30:") && Guid.TryParse(data["u:block30:".Length..], out var b30))
        {
            await _ui.DoBlockPreset(botClient, chatId, messageId, s, b30, days: 30, ct);
            return;
        }

        // u:block_custom:<userId>
        if (data.StartsWith("u:block_custom:") && Guid.TryParse(data["u:block_custom:".Length..], out var bc))
        {
            if (!IsAdmin(s))
            {
                await _ui.EditOrSend(botClient, chatId, messageId, "❌ Siz admin emassiz.", null, ct);
                return;
            }

            s.TargetUserId = bc;
            s.TempDays = null;
            s.Step = BotStep.Admin_Block_CustomDays;

            await _ui.EditOrSend(botClient, chatId, messageId, "🚫 Necha kunga blok qilamiz? (masalan: 5)", null, ct);
            return;
        }

        // u:unblock:<userId>
        if (data.StartsWith("u:unblock:") && Guid.TryParse(data["u:unblock:".Length..], out var ub))
        {
            if (!IsAdmin(s))
            {
                await _ui.EditOrSend(botClient, chatId, messageId, "❌ Siz admin emassiz.", null, ct);
                return;
            }

            var ok = _userRepo.UnblockUser(ub, DateTime.UtcNow);

            await _ui.EditOrSend(botClient, chatId, messageId, ok ? "✅ Unblock qilindi." : "❌ User topilmadi.", null, ct);
            await _ui.ShowUserPanel(botClient, chatId, null, s, ub, ct);
            return;
        }

        // post:view:<guid>
        if (data.StartsWith("post:view:") && Guid.TryParse(data["post:view:".Length..], out var viewId))
        {
            await _ui.ShowPost(botClient, chatId, messageId, s, viewId, ct);
            return;
        }

        // post:del:<guid>
        if (data.StartsWith("post:del:") && Guid.TryParse(data["post:del:".Length..], out var delId))
        {
            if (!IsLoggedIn(s))
            {
                await botClient.SendMessage(chatId, "❌ Avval login qiling.", cancellationToken: ct);
                return;
            }

            // Xohlasang: _auth.EnsureAccess ham tekshirsa bo'ladi
            var access = _auth.EnsureAccess(s.Token);
            if (!access.Success)
            {
                await botClient.SendMessage(chatId, $"❌ {H(access.Error!)}", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, cancellationToken: ct);
                await _ui.SendHome(botClient, chatId, null, s, ct);
                return;
            }

            var ok = _postRepo.Delete(s.Token!.UserId, delId);

            if (!ok)
                await botClient.SendMessage(chatId, "❌ Post topilmadi yoki sizga tegishli emas.", cancellationToken: ct);
            else
                await botClient.SendMessage(chatId, "🗑️ Post o‘chirildi.", cancellationToken: ct);

            await _ui.ShowMyPosts(botClient, chatId, null, s, ct);
            return;
        }

        // fallback
        await _ui.EditOrSend(botClient, chatId, messageId, "❓ Noma'lum buyruq.", null, ct);
    }

    private static bool IsLoggedIn(Session s) => s.Token is not null;

    private static bool IsAdmin(Session s)
    {
        if (s.Token is null) return false;
        var role = s.Token.Role.ToString();
        return role is "Admin" or "SuperAdmin";
    }

    private static string H(string s)
        => (s ?? "").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
}
