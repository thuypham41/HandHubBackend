using System.Net;
using HandHubAPI.Application.Features.Interfaces;
using HandHubAPI.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
namespace HandHubAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : BaseController<NotificationController>
{
    private readonly IChatHubService _chatHubService;
    public NotificationController(
        IChatHubService chatHubService,
        ILogger<NotificationController> logger) : base(logger)
    {
        _chatHubService = chatHubService;
    }

    [HttpGet("get-all-notifications")]
    public async Task<IActionResult> GetAllNotifications([FromQuery] int currentId)
    {
        try
        {
            var notifications = await _chatHubService.GetAllNotificationByCurrentIdAsync(currentId);
            if (!notifications.Any())
                return ErrorResponse("No notifications found", System.Net.HttpStatusCode.NotFound);
            return CommonResponse(notifications, "Notifications retrieved successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to add price negotiation", System.Net.HttpStatusCode.InternalServerError, ex);
        }
    }

}