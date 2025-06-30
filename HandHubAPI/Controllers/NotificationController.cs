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

    [HttpDelete("remove-notification")]
    public async Task<IActionResult> RemoveNotification([FromQuery] int notificationId)
    {
        try
        {
            var result = await _chatHubService.RemoveNotificationAsync(notificationId);
            if (!result)
                return ErrorResponse("Notification not found", HttpStatusCode.NotFound);
            return CommonResponse(result, "Notification removed successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to remove notification", HttpStatusCode.InternalServerError, ex);
        }
    }
}