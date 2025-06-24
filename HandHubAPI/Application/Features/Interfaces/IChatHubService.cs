using HandHubAPI.Application.DTOs;

namespace HandHubAPI.Application.Features.Interfaces
{
    public interface IChatHubService
    {
        // Task<SendMessageRequestDto> AddMessagePersonAsync(SendMessageRequestDto messageViewModel);

        Task<NotificationDto> AddNotificationToUserAsync(NotificationDto notifiationViewModel);

        Task<IEnumerable<SendMessageRequestDto>> GetAllNotificationMessageAsync(string userId);

        Task RemoveMessage(string messageId);

        Task<bool> IsNotificationExist(int senderId, int reciverId);
        Task<IEnumerable<NotificationDto>> GetAllNotificationByCurrentIdAsync(int currentUserId);
    }
}