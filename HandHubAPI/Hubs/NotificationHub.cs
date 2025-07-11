namespace HandHubAPI.Hubs;

using HandHubAPI.Application.DTOs;
using HandHubAPI.Application.Features.Interfaces;
using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using static HandHubAPI.Controllers.PriceNegotiationController;

public class NotificationHub : Hub
{
    private readonly IPriceNegotiationService _priceNegotiationService;
    private readonly IChatHubService _chatHubService;
    private readonly IUnitOfWork _unitOfWork;
    private const string MESSAGE_NOTIFICATION_TITLE = "Cảnh báo!";
    private const string MESSAGE_NOTIFICATION = "Bạn đã nhận được một cảnh báo vi phạm";
    private int _currentUserId;
    public NotificationHub(
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

    // public async Task SendNotificationToUser(int friendId, string message, string title = "Thông báo", string? imageUrl = null)
    // {
    //     var sender = await ValidateCurrentAccount(_currentUserId);

    //     //var reciver = await _userManager.FindByIdAsync(friendId);

    //     var notifiation = await SaveNotificationToUser(sender.Id, friendId, message, title, imageUrl);

    //     await Clients.User(friendId.ToString()).SendAsync("ReceiveNotification", notifiation);
    // }

    public async Task SendNotificationToUser(AddPriceNegotiationRequest request)
    {
        try
        {
            var result = await _priceNegotiationService.AddPriceNegotiationAsync(request);

            // Get product to find seller ID
            var product = await _priceNegotiationService.GetProductByIdAsync(request.ProductId);
            var buyer = await _priceNegotiationService.GetUserByIdAsync(request.BuyerId);
            if (product != null)
            {

                var notificationMessage = $"{buyer?.FullName ?? "Ai đó"} đề xuất giá {request.OfferPrice:C} cho sản phẩm {product.Name}";

                await SaveNotificationToUser(
                    result.Id,
                    request.BuyerId,
                    product.SellerId,
                    notificationMessage,
                    "Đề xuất giá mới",
                    null, request.ProductId);
                await Clients.User(product.SellerId.ToString())
                                    .SendAsync("ReceiveNotification", new
                                    {
                                        SenderId = request.BuyerId,
                                        ReceiverId = product.SellerId,
                                        Message = notificationMessage,
                                        Title = "Đề xuất giá mới",
                                        CreatedAt = DateTime.UtcNow,
                                        ProductId = request.ProductId,
                                        Type = 2
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
       int priceNegotiationId, int senderId, int reciverId, string message, string title, string? imageUrl, int productId)
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
            Type = 1,
            RelatedId = priceNegotiationId,
            ProductId = productId // Assuming ProductId is not used here
        };

        return await _chatHubService.AddNotificationToUserAsync(notificationViewModel);
    }
}