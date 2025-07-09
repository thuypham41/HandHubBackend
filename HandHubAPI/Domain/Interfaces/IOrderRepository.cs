namespace HandHubAPI.Domain.Repositories;

using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;
using HandHubAPI.Infrastructure.Repositories;
using static HandHubAPI.Controllers.OrderController;

public interface IOrderRepository : IBaseRepository<OrderEntity>
{
    Task<IEnumerable<int>> GetPurchasedCategoryIdsByUserAsync(int userId);
    Task<PaginatedResponse<OrderEntity>> GetPaginatedAsync(int pageNumber, int pageSize, int customerId = 0, string? searchTerm = null);
    Task<List<OrderEntity>> GetByIdsAsync(List<int> ids, int status = 3, DateTime? date = null, int currentUserId = 0);
    Task<decimal> GetTotalRevenueAsync(DateTime? startDate, DateTime? endDate);
    Task<List<UserEntity>> GetUsersWithSoldOrdersInDateRange(DateTime? startDate, DateTime? endDate);
    Task<int> GetTotalOrdersByUser(int userId, DateTime? startDate, DateTime? endDate);
    Task<decimal> GetTotalRevenueByUser(int userId, DateTime? startDate, DateTime? endDate);
    Task<decimal> GetTotalRevenueByWeekInMonth(int month, int year, int week);
}