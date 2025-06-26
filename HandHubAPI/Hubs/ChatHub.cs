namespace HandHubAPI.Hubs;

using HandHubAPI.Application.DTOs;
using HandHubAPI.Application.Features.Interfaces;
using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using static HandHubAPI.Controllers.PriceNegotiationController;

public class ChatHub : Hub
{
    private readonly IPriceNegotiationService _priceNegotiationService;
    private readonly IChatHubService _chatHubService;
    private readonly IUnitOfWork _unitOfWork;
    private const string MESSAGE_NOTIFICATION_TITLE = "Cảnh báo!";
    private const string MESSAGE_NOTIFICATION = "Bạn đã nhận được một cảnh báo vi phạm";
    private int _currentUserId;
    public ChatHub(
        IUnitOfWork unitOfWork,
        IChatHubService chatHubService,
        IPriceNegotiationService priceNegotiationService)
    {
        _chatHubService = chatHubService;
        _priceNegotiationService = priceNegotiationService;
        _unitOfWork = unitOfWork;
    }
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var userIdString = httpContext?.Request.Query["userId"];
        if (int.TryParse(userIdString, out _currentUserId))
        {
            await Clients.Others.SendAsync("UserConnected", _currentUserId);
            Console.WriteLine($"User {_currentUserId} đã vào group.");
        }
        else
        {
            Console.WriteLine("Không có userId hợp lệ.");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await Clients.Others.SendAsync("UserDisConnected", _currentUserId);

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageToUser(AddNegotiationMessageRequest request)
    {
        try
        {
            var result = await _priceNegotiationService.AddNegotiationMessageAsync(request);

            var currentPriceNegotiation = await _priceNegotiationService.GetByIdAsync(request.PriceNegotiationId);
            // Get product to find seller ID
            var product = await _priceNegotiationService.GetProductByIdAsync(currentPriceNegotiation.ProductId);
            var buyer = await _priceNegotiationService.GetUserByIdAsync(request.SenderId);
            if (product != null)
            {
                await SaveNotificationToUser(
                    currentPriceNegotiation.Id,
                    request.SenderId,
                    request.ReceiverId,
                    request.MessageContent.Trim(),
                    "Tin nhắn mới từ " + (buyer?.FullName ?? "Người dùng"),
                    null);
                await Clients.User(product.SellerId.ToString())
                                    .SendAsync("ReceiveMessage", new
                                    {
                                        request.SenderId,
                                        request.ReceiverId,
                                        request.MessageContent,
                                        request.PriceNegotiationId,
                                        CreatedAt = DateTime.UtcNow,
                                    });
            }
        }
        catch (Exception ex)
        {
            // Log error or handle appropriately
            Console.WriteLine($"Failed to send notification: {ex.Message}");
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
       int priceNegotiationId, int senderId, int reciverId, string message, string title, string? imageUrl)
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
            Type = 2,
            RelatedId = priceNegotiationId,
        };

        return await _chatHubService.AddNotificationToUserAsync(notificationViewModel);
    }
}