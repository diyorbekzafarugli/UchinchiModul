using System;
using System.Collections.Generic;
using System.Text;

namespace DemoTelegramBot.Bot.Core;

public static class Html
{
    public static string H(string s)
        => (s ?? "").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
}