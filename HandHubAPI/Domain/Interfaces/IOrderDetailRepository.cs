namespace HandHubAPI.Domain.Repositories;

using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;

public interface IOrderDetailRepository : IBaseRepository<OrderDetailEntity>
{
    Task<List<OrderDetailEntity>> GetByOrderIdsAsync(List<int> ids);
    Task<List<OrderDetailEntity>> GetByOrderIdAsync(int id);
    Task<List<OrderDetailEntity>> GetOrderIdsByProductIdsAsync(List<int> productIds);
}