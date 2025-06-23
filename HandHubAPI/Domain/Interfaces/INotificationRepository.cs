namespace HandHubAPI.Domain.Repositories;

using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;

public interface INotificationRepository : IBaseRepository<NotificationEntity>
{
    Task<int> GetTotalRecordByUserIdAsync(int userId);
    Task<bool> IsNotificationExist(int senderId, int reciverId);
    Task<IEnumerable<NotificationEntity>> GetByUserIdPaginatedAsync(int userId, int pageNumber, int pageSize);
    Task<bool> UpdateIsReadNotificationAsync(int id, bool isRead);
    Task<bool> UpdateIsDeleteNotificationAsync(int id, bool isDelete);
    Task<bool> ClearNotificationsAsync(int userId);
}