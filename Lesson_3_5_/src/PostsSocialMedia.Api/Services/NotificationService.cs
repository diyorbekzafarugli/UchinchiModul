using Mapster;
using Microsoft.AspNetCore.SignalR;
using PostsSocialMedia.Api.Dtos.NotificationDto;
using PostsSocialMedia.Api.Entities;
using PostsSocialMedia.Api.Entities.Notification;
using PostsSocialMedia.Api.Hubs;
using PostsSocialMedia.Api.Repositories;

namespace PostsSocialMedia.Api.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        IHubContext<NotificationHub> hubContext)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _hubContext = hubContext;
    }

    public async Task<Result<Guid>> Send(Guid currentUserId, NotificationAddDto dto)
    {
        var userValidation = await InfoUser(currentUserId);
        if (!userValidation.Success) return Result<Guid>.Fail(userValidation.Error!);

        var receiverValidation = await InfoUser(dto.ToUserId);
        if (!receiverValidation.Success) return Result<Guid>.Fail("Qabul qiluvchi: " + receiverValidation.Error);

        if (!Enum.IsDefined(typeof(TypeNotification), dto.Type))
            return Result<Guid>.Fail("Noto'g'ri turdagi bildirishnoma");

        if (string.IsNullOrWhiteSpace(dto.Message))
            return Result<Guid>.Fail("Bo'sh xabar yuborib bo'lmaydi");

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            FromUserId = currentUserId,
            ToUserId = dto.ToUserId,
            Message = dto.Message,
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await _notificationRepository.Add(notification);

        await _hubContext.Clients.User(dto.ToUserId.ToString())
            .SendAsync("ReceiveNotification", new
            {
                notification.Id,
                notification.Message,
                notification.Type,
                FromUserId = currentUserId,
                notification.CreatedAt
            });

        return Result<Guid>.Ok(notification.Id);
    }

    public async Task<Result<List<NotificationGetDto>>> GetFeed(Guid currentUserId, int limit = 20)
    {
        var userCheck = await InfoUser(currentUserId);
        if (!userCheck.Success) return Result<List<NotificationGetDto>>.Fail(userCheck.Error!);

        var notifications = await _notificationRepository.GetByUserId(currentUserId, limit);
        var dtos = notifications.Adapt<List<NotificationGetDto>>();

        return Result<List<NotificationGetDto>>.Ok(dtos);
    }

    public async Task<Result<List<NotificationGetDto>>> GetByType(Guid currentUserId, TypeNotification type)
    {
        var userCheck = await InfoUser(currentUserId);
        if (!userCheck.Success) return Result<List<NotificationGetDto>>.Fail(userCheck.Error!);

        if (!Enum.IsDefined(typeof(TypeNotification), type))
            return Result<List<NotificationGetDto>>.Fail("Noto'g'ri turdagi bildirishnoma");

        var notifications = await _notificationRepository.GetByUserIdAndType(currentUserId, type);
        var dtos = notifications.Adapt<List<NotificationGetDto>>();

        return Result<List<NotificationGetDto>>.Ok(dtos);
    }

    public async Task<Result<int>> GetNotificationCount(Guid currentUserId)
    {
        var userCheck = await InfoUser(currentUserId);
        if (!userCheck.Success) return Result<int>.Fail(userCheck.Error!);

        var count = await _notificationRepository.GetUnreadCount(currentUserId);
        return Result<int>.Ok(count);
    }

    public async Task<Result<bool>> MarkAsRead(Guid currentUserId, Guid id)
    {
        var notification = await _notificationRepository.GetById(id);
        if (notification == null) return Result<bool>.Fail("Bildirishnoma topilmadi");

        if (notification.ToUserId != currentUserId)
            return Result<bool>.Fail("Bu bildirishnoma sizga tegishli emas");

        notification.IsRead = true;
        await _notificationRepository.Update(notification);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> Delete(Guid currentUserId, Guid id)
    {
        var notification = await _notificationRepository.GetById(id);
        if (notification == null) return Result<bool>.Fail("Bildirishnoma topilmadi");

        if (notification.ToUserId != currentUserId)
            return Result<bool>.Fail("Sizga tegishli bo'lmagan bildirishnomani o'chira olmaysiz");

        await _notificationRepository.Delete(id);
        return Result<bool>.Ok(true);
    }

    private async Task<Result<bool>> InfoUser(Guid userId)
    {
        if (userId == Guid.Empty)
            return Result<bool>.Fail("Foydalanuvchi ID si xato");

        var userFromDB = await _userRepository.GetById(userId);
        if (userFromDB == null)
            return Result<bool>.Fail("Foydalanuvchi topilmadi");

        if (userFromDB.IsBlocked)
            return Result<bool>.Fail("Foydalanuvchi bloklangan");

        return Result<bool>.Ok(true);
    }
}