using HandHubAPI.Application.DTOs;
using HandHubAPI.Application.Features.Interfaces;
using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;

namespace HandHubAPI.Application.Features.Implements;

public class ChatHubService : IChatHubService
{
    private readonly ILogger<ChatHubService> _logger;
    private readonly IUnitOfWork _uow;

    public ChatHubService(
        ILogger<ChatHubService> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _uow = unitOfWork;
    }

    // public async Task<SendMessageRequestDto> AddMessagePersonAsync(SendMessageRequestDto request)
    // {
    //     try
    //     {
    //         var validateResult = ValidateMessage(request);
    //         var newMessage = InitMessage(request);
    //         if (validateResult)
    //         {
    //             await _uow.MessageRepository.AddAsync(newMessage);
    //             await _uow.CommitAsync();
    //             request.MessageId = newMessage.Id;

    //             return request;
    //         }
    //         else
    //             return null;
    //     }
    //     catch (Exception e)
    //     {
    //         return null;
    //     }
    // }



    public async Task<NotificationDto> AddNotificationToUserAsync(NotificationDto request)
    {
        try
        {
            var entity = new NotificationEntity
            {
                Title = request.Title ?? "",
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Content = request.Messeage ?? "",
                Type = request.Type,
                RelatedId = request.RelatedId,
                ProductId = request.ProductId, // Assuming ProductId is part of the NotificationDto
                                               // Assuming RelatedId is optional
            };

            var notification = await _uow.NotificationRepository.AddAsync(entity);
            await _uow.CommitAsync();

            request.Id = notification.Id;

            return request;

        }
        catch (Exception e)
        {
            var x = e.Message;
            return new NotificationDto();
        }
    }

    public Task<IEnumerable<SendMessageRequestDto>> GetAllNotificationMessageAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsNotificationExist(int senderId, int reciverId)
    {
        return await _uow.NotificationRepository.IsNotificationExist(senderId, reciverId);
    }

    public async Task<IEnumerable<NotificationDto>> GetAllNotificationByCurrentIdAsync(int currentId)
    {
        try
        {
            var notifications = await _uow.NotificationRepository.GetAllNotificationByCurrentIdAsync(currentId);
            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                SenderId = n.SenderId,
                ReceiverId = n.ReceiverId,
                Messeage = n.Content,
                Type = n.Type,
                RelatedId = n.RelatedId,
                ProductId = n.ProductId,
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting notifications for user {CurrentId}", currentId);
            return new List<NotificationDto>();
        }
    }

    public Task RemoveMessage(string messageId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RemoveNotificationAsync(int notificationId)
    {
        try
        {
            var notification = await _uow.NotificationRepository.GetByIdAsync(notificationId);
            if (notification == null)
                return false;

            notification.IsDeleted = true; // Mark as deleted instead of removing
            _uow.NotificationRepository.Update(notification); // Update the entity state
            await _uow.CommitAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error removing notification {NotificationId}", notificationId);
            return false;
        }
    }

    private bool ValidateMessage(SendMessageRequestDto messageRequest)
    {
        if (messageRequest.SenderId == 0)
            return false;
        if (string.IsNullOrEmpty(messageRequest.Content))
            return false;
        return true;
    }

    // private MessageEntity InitMessage(SendMessageRequestDto messageRequest)
    // {
    //     return new MessageEntity
    //     {
    //         Id = Guid.NewGuid().ToString(),
    //         SenderId = messageRequest.SenderId,
    //         MatchId = messageRequest.MatchId,
    //         Content = messageRequest.Content,
    //         CreatedAt = messageRequest.SendTime,
    //         IsImage = messageRequest.IsImage,
    //     };
    // }
}
