using DemoTelegramBot.Bot.Core;
using DemoTelegramBot.Repositories;
using DemoTelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DemoTelegramBot.Bot.UI;

public sealed class BotUi
{
    private readonly BotContext _ctx;
    private readonly IAuthService _auth;
    private readonly IUserRepository _userRepo;
    private readonly IPostRepository _postRepo;

    public BotUi(BotContext ctx, IAuthService auth, IUserRepository userRepo, IPostRepository postRepo)
    {
        _ctx = ctx;
        _auth = auth;
        _userRepo = userRepo;
        _postRepo = postRepo;
    }

    public async Task SendHome(ITelegramBotClient botClient, long chatId, int? messageId, Session s, CancellationToken ct)
    {
        var text =
            "<b>🏠 MySocialMedia Bot</b>\n\n" +
            (s.Token is null
                ? "Holat: <b>Guest</b>\n\nLogin yoki Register qiling."
                : $"Holat: <b>Logged in</b>\nUserId: <code>{s.Token.UserId}</code>\nRole: <b>{s.Token.Role}</b>");

        var kb = HomeKeyboard(s);
        await EditOrSend(botClient, chatId, messageId, text, kb, ct);
    }

    public InlineKeyboardMarkup HomeKeyboard(Session s)
    {
        if (s.Token is null)
        {
            return new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("🆕 Register", "auth:register"),
                    InlineKeyboardButton.WithCallbackData("🔐 Login", "auth:login"),
                }
            });
        }

        var rows = new List<InlineKeyboardButton[]>
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData("📝 Post qo‘shish", "post:add"),
                InlineKeyboardButton.WithCallbackData("📚 Postlarim", "post:my"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData("🚪 Logout", "auth:logout"),
                InlineKeyboardButton.WithCallbackData("🏠 Home", "nav:home"),
            }
        };

        // Admin tekshiruv (2 xil yo‘l)
        // 1) Agar IAuthService’da IsAdmin bo‘lsa:
        // if (_auth.IsAdmin(s.Token)) { ... }

        // 2) Agar yo‘q bo‘lsa, Token.Role orqali:
        if (IsAdminToken(s.Token))
        {
            rows.Insert(1, new[]
            {
                InlineKeyboardButton.WithCallbackData("📰 All posts (Admin)", "admin:posts_all"),
                InlineKeyboardButton.WithCallbackData("🛡️ Users (Admin)", "admin:users"),
            });
        }

        return new InlineKeyboardMarkup(rows);
    }

    public async Task EditOrSend(ITelegramBotClient botClient, long chatId, int? messageId, string text, InlineKeyboardMarkup? kb, CancellationToken ct)
    {
        if (messageId is int mid)
        {
            try
            {
                await botClient.EditMessageText(chatId, mid, text, parseMode: ParseMode.Html, replyMarkup: kb, cancellationToken: ct);
                return;
            }
            catch { /* mayda xatolarni o'tkazib yuboramiz */ }
        }

        await botClient.SendMessage(chatId, text, parseMode: ParseMode.Html, replyMarkup: kb, cancellationToken: ct);
    }

    public async Task ShowMyPosts(ITelegramBotClient botClient, long chatId, int? messageId, Session s, CancellationToken ct)
    {
        var access = _auth.EnsureAccess(s.Token);
        if (!access.Success)
        {
            await EditOrSend(botClient, chatId, messageId, $"❌ {H(access.Error!)}", HomeKeyboard(s), ct);
            return;
        }

        var posts = _postRepo.GetByUserId(s.Token!.UserId)
                             .OrderByDescending(p => p.CreatedAt)
                             .Take(10)
                             .ToList();

        var text = $"<b>📚 Postlarim</b> (top 10)\n\n";
        if (posts.Count == 0)
        {
            text += "Hozircha post yo‘q.";
            await EditOrSend(botClient, chatId, messageId, text,
                new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("⬅️ Orqaga", "nav:home") }
                }), ct);
            return;
        }

        foreach (var p in posts)
            text += $"• <b>{H(p.Title)}</b>\n<code>{p.PostId}</code>\n\n";

        var buttons = posts.Select(p => new[]
        {
            InlineKeyboardButton.WithCallbackData("👁 Ko‘rish", $"post:view:{p.PostId}"),
            InlineKeyboardButton.WithCallbackData("🗑 O‘chirish", $"post:del:{p.PostId}"),
        }).ToList();

        buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("⬅️ Orqaga", "nav:home") });

        await EditOrSend(botClient, chatId, messageId, text, new InlineKeyboardMarkup(buttons), ct);
    }

    public async Task ShowPost(ITelegramBotClient botClient, long chatId, int messageId, Session s, Guid postId, CancellationToken ct)
    {
        var access = _auth.EnsureAccess(s.Token);
        if (!access.Success)
        {
            await EditOrSend(botClient, chatId, messageId, $"❌ {H(access.Error!)}", HomeKeyboard(s), ct);
            return;
        }

        var p = _postRepo.GetById(s.Token!.UserId, postId);
        if (p is null)
        {
            await EditOrSend(botClient, chatId, messageId, "❌ Post topilmadi.", null, ct);
            return;
        }

        var text =
            $"<b>📝 {H(p.Title)}</b>\n\n" +
            $"{H(p.Content)}\n\n" +
            $"ID: <code>{p.PostId}</code>\n" +
            $"Created: <code>{p.CreatedAt:u}</code>";

        var kb = new InlineKeyboardMarkup(new[]
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData("🗑 O‘chirish", $"post:del:{p.PostId}"),
                InlineKeyboardButton.WithCallbackData("⬅️ Postlarim", "post:my")
            }
        });

        await EditOrSend(botClient, chatId, messageId, text, kb, ct);
    }

    public async Task ShowUsers(ITelegramBotClient botClient, long chatId, int? messageId, Session s, CancellationToken ct)
    {
        if (!IsAdminToken(s.Token))
        {
            await EditOrSend(botClient, chatId, messageId, "❌ Siz admin emassiz.", null, ct);
            return;
        }

        var users = _userRepo.GetAll()
                             .OrderByDescending(u => u.RegisteredAt)
                             .Take(12)
                             .ToList();

        var text = "<b>🛡️ Users (top 12)</b>\n\n";
        foreach (var u in users)
        {
            var blocked = u.IsBlocked ? "🚫 Blocked" : "✅ Active";
            text += $"• <b>{H(u.UserName)}</b> | {blocked} | Role: <b>{u.Role}</b>\n<code>{u.UserId}</code>\n\n";
        }

        var buttons = users.Select(u => new[]
        {
            InlineKeyboardButton.WithCallbackData($"👤 {u.UserName}", $"u:open:{u.UserId}")
        }).ToList();

        buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("⬅️ Orqaga", "nav:home") });

        await EditOrSend(botClient, chatId, messageId, text, new InlineKeyboardMarkup(buttons), ct);
    }

    public async Task ShowAllPosts(ITelegramBotClient botClient, long chatId, int? messageId, Session s, CancellationToken ct)
    {
        if (!IsAdminToken(s.Token))
        {
            await EditOrSend(botClient, chatId, messageId, "❌ Siz admin emassiz.", null, ct);
            return;
        }

        var posts = _postRepo.GetAll()
                             .OrderByDescending(p => p.CreatedAt)
                             .Take(10)
                             .ToList();

        var text = "<b>📰 All posts (top 10)</b>\n\n";
        if (posts.Count == 0)
        {
            text += "Post yo‘q.";
            await EditOrSend(botClient, chatId, messageId, text,
                new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("⬅️ Orqaga", "nav:home") }
                }), ct);
            return;
        }

        foreach (var p in posts)
            text += $"• <b>{H(p.Title)}</b>\nUser: <code>{p.UserId}</code>\nPostId: <code>{p.PostId}</code>\n\n";

        await EditOrSend(botClient, chatId, messageId, text,
            new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("⬅️ Orqaga", "nav:home") }
            }), ct);
    }

    public async Task ShowUserPosts(ITelegramBotClient botClient, long chatId, int? messageId, Session s, Guid userId, CancellationToken ct)
    {
        if (!IsAdminToken(s.Token))
        {
            await EditOrSend(botClient, chatId, messageId, "❌ Siz admin emassiz.", null, ct);
            return;
        }

        var posts = _postRepo.GetByUserId(userId)
                             .OrderByDescending(p => p.CreatedAt)
                             .Take(10)
                             .ToList();

        var text = $"<b>📚 User postlari</b>\nUserId: <code>{userId}</code>\n\n";

        if (posts.Count == 0)
        {
            text += "Post yo‘q.";
            await EditOrSend(botClient, chatId, messageId, text,
                new InlineKeyboardMarkup(new[]
                {
                new[] { InlineKeyboardButton.WithCallbackData("⬅️ User panel", $"u:open:{userId}") }
                }), ct);
            return;
        }

        foreach (var p in posts)
        {
            text += $"• <b>{H(p.Title)}</b>\n" +
                    $"PostId: <code>{p.PostId}</code>\n" +
                    $"Created: <code>{p.CreatedAt:u}</code>\n\n";
        }

        await EditOrSend(botClient, chatId, messageId, text,
            new InlineKeyboardMarkup(new[]
            {
            new[] { InlineKeyboardButton.WithCallbackData("⬅️ User panel", $"u:open:{userId}") }
            }), ct);
    }

    public async Task ShowUserPanel(ITelegramBotClient botClient, long chatId, int? messageId, Session s, Guid userId, CancellationToken ct)
    {
        if (!IsAdminToken(s.Token))
        {
            await EditOrSend(botClient, chatId, messageId, "❌ Siz admin emassiz.", null, ct);
            return;
        }

        var u = _userRepo.GetById(userId);
        if (u is null)
        {
            await EditOrSend(botClient, chatId, messageId, "❌ User topilmadi.", null, ct);
            return;
        }

        var status = u.IsBlocked ? "🚫 Blocked" : "✅ Active";
        var until = u.BlockedUntil is null ? "-" : u.BlockedUntil.Value.ToString("u");
        var reason = string.IsNullOrWhiteSpace(u.BlockReason) ? "-" : H(u.BlockReason);

        var text =
            $"<b>👤 User panel</b>\n\n" +
            $"Username: <b>{H(u.UserName)}</b>\n" +
            $"UserId: <code>{u.UserId}</code>\n" +
            $"Role: <b>{u.Role}</b>\n" +
            $"Status: <b>{status}</b>\n" +
            $"BlockedUntil: <code>{until}</code>\n" +
            $"Reason: {reason}\n";

        var kb = new InlineKeyboardMarkup(new[]
        {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("📚 Postlari", $"u:posts:{u.UserId}"),
            InlineKeyboardButton.WithCallbackData("✅ Unblock", $"u:unblock:{u.UserId}")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("🚫 Block 7d", $"u:block7:{u.UserId}"),
            InlineKeyboardButton.WithCallbackData("🚫 Block 30d", $"u:block30:{u.UserId}")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("🧾 Block (custom)", $"u:block_custom:{u.UserId}"),
            InlineKeyboardButton.WithCallbackData("⬅️ Users", "admin:users")
        }
    });

        await EditOrSend(botClient, chatId, messageId, text, kb, ct);
    }

    public async Task DoBlockPreset(ITelegramBotClient botClient, long chatId, int messageId, Session s, Guid userId, int days, CancellationToken ct)
    {
        if (!IsAdminToken(s.Token))
        {
            await EditOrSend(botClient, chatId, messageId, "❌ Siz admin emassiz.", null, ct);
            return;
        }

        var now = DateTime.UtcNow;
        var until = now.AddDays(days);

        var ok = _userRepo.BlockUser(userId, $"Admin blokladi ({days} kun)", now, until);

        await EditOrSend(botClient, chatId, messageId,
            ok ? $"✅ {days} kunga block qilindi." : "❌ User topilmadi.",
            null, ct);

        await ShowUserPanel(botClient, chatId, null, s, userId, ct);
    }


    private static bool IsAdminToken(dynamic? token)
    {
        // Token.Role enum bo‘lsa shu ishlaydi: "Admin"/"SuperAdmin"
        if (token is null) return false;
        var role = token.Role?.ToString();
        return role is "Admin" or "SuperAdmin";
    }

    private static string H(string s)
        => (s ?? "").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
}
