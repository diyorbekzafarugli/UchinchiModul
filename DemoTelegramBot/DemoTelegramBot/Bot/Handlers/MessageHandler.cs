using DemoTelegramBot.Bot.Core;
using DemoTelegramBot.Bot.UI;
using DemoTelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DemoTelegramBot.Bot.Handlers;

public sealed class MessageHandler : IMessageHandler
{
    private readonly BotContext _ctx;
    private readonly IAuthService _auth;
    private readonly IPostService _posts;
    private readonly AdminService _admin;
    private readonly BotUi _ui;

    public MessageHandler(BotContext ctx, IAuthService auth, IPostService posts, AdminService admin, BotUi ui)
    {
        _ctx = ctx;
        _auth = auth;
        _posts = posts;
        _admin = admin;
        _ui = ui;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, Message msg, CancellationToken ct)
    {
        var chatId = msg.Chat.Id;
        var tgUserId = msg.From!.Id;
        var text = (msg.Text ?? "").Trim();

        var s = _ctx.GetSession(tgUserId);

        if (text is "/start" or "/menu")
        {
            s.Step = BotStep.None;
            await _ui.SendHome(botClient, chatId, null, s, ct);
            return;
        }

        // ------------------ Step flows ------------------

        if (s.Step == BotStep.Register_UserName)
        {
            s.TempUserName = text;
            s.Step = BotStep.Register_Password;
            await botClient.SendMessage(chatId, "🔑 Parol yuboring (kamida 6 ta):", cancellationToken: ct);
            return;
        }

        if (s.Step == BotStep.Register_Password)
        {
            s.TempPassword = text;
            s.Step = BotStep.Register_FullName;
            await botClient.SendMessage(chatId, "👤 FullName yuboring:", cancellationToken: ct);
            return;
        }

        if (s.Step == BotStep.Register_FullName)
        {
            s.TempFullName = text;
            s.Step = BotStep.Register_DateOfBirth;
            await botClient.SendMessage(chatId, "🎂 Tug‘ilgan sana yuboring (masalan: 2000-01-31):", cancellationToken: ct);
            return;
        }

        if (s.Step == BotStep.Register_DateOfBirth)
        {
            if (!DateTime.TryParse(text, out var dob))
            {
                await botClient.SendMessage(chatId, "❌ Sana noto‘g‘ri. Masalan: 2000-01-31", cancellationToken: ct);
                return;
            }

            var dto = new DemoTelegramBot.Dtos.UserRegisterDto
            {
                UserName = s.TempUserName!,
                Password = s.TempPassword!,
                FullName = s.TempFullName!,
                DateOfBirth = dob
            };

            var res = _auth.Register(dto);

            if (!res.Success)
            {
                s.Step = BotStep.None;
                await botClient.SendMessage(chatId, $"❌ {H(res.Error!)}", parseMode: ParseMode.Html, cancellationToken: ct);
                await _ui.SendHome(botClient, chatId, null, s, ct);
                return;
            }

            s.Token = res.Data!;
            s.Step = BotStep.None;

            // temp’larni tozalab qo‘ysang ham bo‘ladi
            s.TempUserName = null; s.TempPassword = null; s.TempFullName = null; s.TempDateOfBirth = null;

            await botClient.SendMessage(chatId, "✅ Ro‘yxatdan o‘tildi va login bo‘ldi.", cancellationToken: ct);
            await _ui.SendHome(botClient, chatId, null, s, ct);
            return;
        }

        if (s.Step == BotStep.Login_UserName)
        {
            s.TempUserName = text;
            s.Step = BotStep.Login_Password;
            await botClient.SendMessage(chatId, "🔑 Parol yuboring:", cancellationToken: ct);
            return;
        }

        if (s.Step == BotStep.Login_Password)
        {
            var res = _auth.Login(s.TempUserName!, text);

            if (!res.Success)
            {
                s.Step = BotStep.None;
                await botClient.SendMessage(chatId, $"❌ {H(res.Error!)}", parseMode: ParseMode.Html, cancellationToken: ct);
                await _ui.SendHome(botClient, chatId, null, s, ct);
                return;
            }

            s.Token = res.Data!;
            s.Step = BotStep.None;
            await botClient.SendMessage(chatId, "✅ Login bo‘ldi.", cancellationToken: ct);
            await _ui.SendHome(botClient, chatId, null, s, ct);
            return;
        }

        if (s.Step == BotStep.AddPost_Title)
        {
            s.TempTitle = text;
            s.Step = BotStep.AddPost_Content;
            await botClient.SendMessage(chatId, "📝 Post matnini yuboring:", cancellationToken: ct);
            return;
        }

        if (s.Step == BotStep.AddPost_Content)
        {
            var access = _auth.EnsureAccess(s.Token);

            if (!access.Success)
            {
                s.Step = BotStep.None;
                await botClient.SendMessage(chatId, $"❌ {H(access.Error!)}", parseMode: ParseMode.Html, cancellationToken: ct);
                await _ui.SendHome(botClient, chatId, null, s, ct);
                return;
            }


            var add = _posts.Add(s.Token!, s.TempTitle!, text);

            s.Step = BotStep.None;

            if (!add.Success)
                await botClient.SendMessage(chatId, $"❌ {H(add.Error!)}", parseMode: ParseMode.Html, cancellationToken: ct);
            else
                await botClient.SendMessage(chatId, $"✅ Post qo‘shildi. ID: <code>{add.Data}</code>", parseMode: ParseMode.Html, cancellationToken: ct);

            await _ui.SendHome(botClient, chatId, null, s, ct);
            return;
        }

        if (s.Step == BotStep.Admin_Block_CustomDays)
        {
            if (!IsAdmin(s) || s.TargetUserId is null)
            {
                s.Step = BotStep.None;
                await botClient.SendMessage(chatId, "❌ Admin huquqi yo‘q.", cancellationToken: ct);
                return;
            }

            if (!int.TryParse(text, out var days) || days < 1 || days > 365)
            {
                await botClient.SendMessage(chatId, "❌ Noto‘g‘ri. 1..365 oralig‘ida son kiriting:", cancellationToken: ct);
                return;
            }

            s.TempDays = days;
            s.Step = BotStep.Admin_Block_CustomReason;
            await botClient.SendMessage(chatId, "🧾 Sabab yozing (yoki '-' deb yuboring):", cancellationToken: ct);
            return;
        }

        if (s.Step == BotStep.Admin_Block_CustomReason)
        {
            if (!IsAdmin(s) || s.TargetUserId is null || s.TempDays is null)
            {
                s.Step = BotStep.None;
                await botClient.SendMessage(chatId, "❌ Admin huquqi yo‘q.", cancellationToken: ct);
                return;
            }

            var reason = text == "-" ? null : text;

            var now = DateTime.UtcNow;
            var until = now.AddDays(s.TempDays.Value);

            // ⚠️ AdminService method nomi sende boshqacha bo‘lishi mumkin
            var ok = _admin.BlockUser(s.TargetUserId.Value, reason, now, until);

            s.Step = BotStep.None;

            await botClient.SendMessage(chatId, ok ? "✅ Block qilindi." : "❌ User topilmadi.", cancellationToken: ct);

            await _ui.ShowUserPanel(botClient, chatId, null, s, s.TargetUserId.Value, ct);
            return;
        }

        // Default
        await botClient.SendMessage(chatId, "Menyudan foydalaning: /menu", cancellationToken: ct);
    }

    private static bool IsAdmin(Session s)
    {
        if (s.Token is null) return false;
        var role = s.Token.Role.ToString();
        return role is "Admin" or "SuperAdmin";
    }

    private static string H(string s)
        => (s ?? "").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
}
