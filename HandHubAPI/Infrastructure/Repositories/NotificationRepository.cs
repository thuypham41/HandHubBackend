using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HandHubAPI.Infrastructure.Repositories;

public class NotificationRepository : BaseRepository<NotificationEntity>, INotificationRepository
{
    public NotificationRepository(HandHubDbContext context) : base(context)
    {
    }
    private const int DEFAULT_PAGE_SIZE = 30;

    public async Task<bool> ClearNotificationsAsync(int userId)
    {
        var notifications = await GetAllAsync();
        if (notifications == null)
            return false;
        foreach (var item in notifications)
        {
            if (item.ReceiverId == userId && !item.IsDeleted)
            {
                item.IsDeleted = true;
                item.UpdatedAt = DateTime.UtcNow;
                Update(item);
            }
        }

        return true;
    }

    public async Task<IEnumerable<NotificationEntity>> GetByUserIdPaginatedAsync
        (int userId, int pageNumber, int pageSize = DEFAULT_PAGE_SIZE)
    {
        var validPageNumber = Math.Max(1, pageNumber);
        return await _context.Notification
            .Where(x => x.ReceiverId == userId && !x.IsDeleted)
            .OrderBy(x => x.IsRead)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((validPageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalRecordByUserIdAsync(int userId)
    {
        return await _context.Notification.Where(x => x.ReceiverId == userId).CountAsync();
    }

    public async Task<bool> IsNotificationExist(int senderId, int reciverId)
    {
        var notification = await _context.Notification
            .Where(n => n.ReceiverId == reciverId &&
                        (senderId != 0 || n.SenderId == senderId) &&
                        !n.IsRead &&
                        !n.IsDeleted)
            .FirstOrDefaultAsync();

        return notification == null ? false : true;
    }

    public async Task<bool> UpdateIsDeleteNotificationAsync(int id, bool isDelete)
    {
        var notification = await GetByIdAsync(id);
        if (notification == null)
            return false;
        notification.IsDeleted = isDelete;
        notification.UpdatedAt = DateTime.UtcNow;
        Update(notification);

        return true;
    }

    public async Task<bool> UpdateIsReadNotificationAsync(int id, bool isRead)
    {
        var notification = await GetByIdAsync(id);
        if (notification == null)
            return false;
        notification.IsRead = isRead;
        notification.UpdatedAt = DateTime.UtcNow;
        Update(notification);

        return true;
    }

    public async Task<IEnumerable<NotificationEntity>> GetAllNotificationByCurrentIdAsync(int userId)
    {
        return await _context.Notification
            .Where(x => x.ReceiverId == userId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
}