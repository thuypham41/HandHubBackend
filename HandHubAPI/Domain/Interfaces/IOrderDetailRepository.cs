namespace HandHubAPI.Domain.Repositories;

using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;

public interface IOrderDetailRepository : IBaseRepository<OrderDetailEntity>
{
    Task<List<OrderDetailEntity>> GetByOrderIdsAsync(List<int> ids);
}