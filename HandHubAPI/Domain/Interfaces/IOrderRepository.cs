namespace HandHubAPI.Domain.Repositories;

using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;

public interface IOrderRepository : IBaseRepository<OrderEntity>
{
    Task<IEnumerable<int>> GetPurchasedCategoryIdsByUserAsync(int userId);
}