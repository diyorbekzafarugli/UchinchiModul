using DemoTelegramBot.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoTelegramBot.Services;

public sealed class AdminService
{
    private readonly IUserRepository _userRepo;

    public AdminService(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public bool BlockUser(Guid userId, string? reason, DateTime now, DateTime until)
        => _userRepo.BlockUser(userId, reason, now, until);

    public bool UnblockUser(Guid userId, DateTime now)
        => _userRepo.UnblockUser(userId, now);
}
