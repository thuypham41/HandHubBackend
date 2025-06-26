using System.Net;
using HandHubAPI.Application.Features.Interfaces;
using HandHubAPI.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
namespace HandHubAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PriceNegotiationController : BaseController<PriceNegotiationController>
{
    private readonly IPriceNegotiationService _priceNegotiationService;
    private readonly IHubContext<NotificationHub> _notificationHubContext;
    private readonly IChatHubService _chatHubService;
    public PriceNegotiationController(
        IPriceNegotiationService priceNegotiationService,
        IChatHubService chatHubService,
        IHubContext<NotificationHub> notificationHubContext,
        ILogger<PriceNegotiationController> logger) : base(logger)
    {
        _priceNegotiationService = priceNegotiationService;
        _chatHubService = chatHubService;
        _notificationHubContext = notificationHubContext;
    }

    [HttpPost("add-price-negotiation")]
    public async Task<IActionResult> AddPriceNegotiationAsync([FromBody] AddPriceNegotiationRequest request)
    {
        try
        {
            var result = await _priceNegotiationService.AddPriceNegotiationAsync(request);
            // if (result == null)
            // {
            //     return ErrorResponse("Price negotiation existed!", HttpStatusCode.NotFound);
            // }

            // Get product to find seller ID
            var product = await _priceNegotiationService.GetProductByIdAsync(request.ProductId);
            var buyer = await _priceNegotiationService.GetUserByIdAsync(request.BuyerId);
            if (product != null)
            {
                // Send notification to seller about new price negotiation
                var notificationHub = HttpContext.RequestServices.GetService<IHubContext<NotificationHub>>();
                if (notificationHub != null)
                {
                    var notificationMessage = $"{buyer?.FullName ?? "Ai đó"} đề xuất giá {request.OfferPrice:C} cho sản phẩm {product.Name}";

                    await SaveNotificationToUser(
                        request.BuyerId,
                        product.SellerId,
                        notificationMessage,
                        "Đề xuất giá mới",
                        null);
                    await _notificationHubContext.Clients.User(product.SellerId.ToString())
                        .SendAsync("ReceiveNotification", new
                        {
                            SenderId = request.BuyerId,
                            ReceiverId = product.SellerId,
                            Message = notificationMessage,
                            Title = "Đề xuất giá mới",
                            CreatedAt = DateTime.UtcNow,
                            Type = 2
                        });
                }
            }

            return CommonResponse(result, "Price negotiation added successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to add price negotiation", System.Net.HttpStatusCode.InternalServerError, ex);
        }
    }

    private const string MESSAGE_NOTIFICATION_TITLE = "Thông báo!";
    private const string MESSAGE_NOTIFICATION = "Bạn đã nhận được đề nghị thương lượng giá mới";
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

    [HttpGet("get-all-messages")]
    public async Task<IActionResult> GetNegotiationMessagesAsync(int priceNegotiationId)
    {
        try
        {
            var messages = await _priceNegotiationService.GetAllMessagesAsync(priceNegotiationId);
            return CommonResponse(messages, "Messages retrieved successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to retrieve messages", HttpStatusCode.InternalServerError, ex);
        }
    }

    public class AddNegotiationMessageRequest
    {
        public int PriceNegotiationId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string MessageContent { get; set; } = string.Empty;
    }

    public class AddPriceNegotiationRequest
    {
        public int ProductId { get; set; }
        public decimal OfferPrice { get; set; }
        public int BuyerId { get; set; }
        public string? SellerResponse { get; set; }
    }
}