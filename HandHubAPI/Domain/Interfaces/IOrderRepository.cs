namespace HandHubAPI.Domain.Repositories;

using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;
using HandHubAPI.Infrastructure.Repositories;

public interface IOrderRepository : IBaseRepository<OrderEntity>
{
    Task<IEnumerable<int>> GetPurchasedCategoryIdsByUserAsync(int userId);
    Task<PaginatedResponse<OrderEntity>> GetPaginatedAsync(int pageNumber, int pageSize, int customerId = 0, string? searchTerm = null);
    Task<List<OrderEntity>> GetByIdsAsync(List<int> ids, int status = 3, DateTime? date = null, int currentUserId = 0);
}