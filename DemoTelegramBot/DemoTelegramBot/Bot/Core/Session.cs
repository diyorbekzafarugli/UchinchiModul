using DemoTelegramBot.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoTelegramBot.Bot.Core;

public sealed class Session
{
    public Token? Token { get; set; }

    public string? TempPassword { get; set; }
    public string? TempFullName { get; set; }
    public DateTime? TempDateOfBirth { get; set; }

    public BotStep Step { get; set; } = BotStep.None;
    public Guid? TargetUserId { get; set; }
    public int? TempDays { get; set; }

    public string? TempUserName { get; set; }
    public string? TempTitle { get; set; }
}

public enum BotStep
{
    None,
    Register_UserName,
    Register_Password,
    Register_FullName,
    Register_DateOfBirth,
    Login_UserName,
    Login_Password,
    AddPost_Title,
    AddPost_Content,
    Admin_Block_CustomDays,
    Admin_Block_CustomReason
}
