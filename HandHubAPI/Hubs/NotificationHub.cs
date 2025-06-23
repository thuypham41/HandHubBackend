namespace HandHubAPI.Hubs;

using HandHubAPI.Application.DTOs;
using HandHubAPI.Application.Features.Interfaces;
using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
    private readonly IChatHubService _chatHubService;
    private readonly IUnitOfWork _unitOfWork;
    private const string MESSAGE_NOTIFICATION_TITLE = "Cảnh báo!";
    private const string MESSAGE_NOTIFICATION = "Bạn đã nhận được một cảnh báo vi phạm";
    private int _currentUserId;
    public NotificationHub(
        IUnitOfWork unitOfWork,
        IChatHubService chatHubService)
    {
        _chatHubService = chatHubService;
        _unitOfWork = unitOfWork;
    }
    public async Task OnConnectedAsync(int userId)
    {
        _currentUserId = userId;
        await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await Clients.Others.SendAsync("UserDisConnected", _currentUserId);

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendNotificationToUser(int friendId, string message, string title = "Thông báo", string? imageUrl = null)
    {
        var sender = await ValidateCurrentAccount(_currentUserId);

        //var reciver = await _userManager.FindByIdAsync(friendId);

        var notifiation = await SaveNotificationToUser(sender.Id, friendId, message, title, imageUrl);

        await Clients.User(friendId.ToString()).SendAsync("ReceiveNotification", notifiation);
    }

    public async Task<bool> SendWarningNotificationReportToUser(SendMessageRequestDto param)
    {
        var sender = await ValidateCurrentAccount(param.SenderId);

        param.Content = param.Content.Trim();

        try
        {
            var isNotificationExist = await _chatHubService.IsNotificationExist(sender.Id, param.ReceiverId);

            if (!isNotificationExist)
            {
                var notification = await SaveNotificationToUser(
                    sender.Id, param.ReceiverId, param?.Content ?? MESSAGE_NOTIFICATION,
                    "Cảnh báo từ quản trị viên", "https://example.com/warning-image.png");

                await Clients.User(param.ReceiverId.ToString()).SendAsync("ReceiveNotification", notification);
            }

            return true;
        }
        catch (Exception c)
        {

            return false;
        }
    }

    private async Task<UserEntity> ValidateCurrentAccount(int id)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(id);

        if (user == null)
        {
            await Clients.Caller.SendAsync("UserNotConnected", "You must login to chat!");

            Context.Abort();

            throw new Exception("UserNotConnected!");
        }

        return user;
    }

    private async Task<NotificationDto> SaveNotificationToUser(
       int senderId, int reciverId, string message, string title, string? imageUrl)
    {
        var sendDatetime = DateTime.UtcNow;

        var notificationViewModel = new NotificationDto
        {
            SenderId = senderId,
            ReceiverId = reciverId,
            Messeage = message,
            CreatedAt = sendDatetime,
            UpdatedAt = sendDatetime,
            Title = title,
            Subtitle = MESSAGE_NOTIFICATION_TITLE,
            Type = 1
        };

        return await _chatHubService.AddNotificationToUserAsync(notificationViewModel);
    }
}