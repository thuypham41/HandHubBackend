namespace HandHubAPI.Domain.Repositories;

using HandHubAPI.Application.DTOs;
using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;

public interface IOrderDetailRepository : IBaseRepository<OrderDetailEntity>
{
    Task<List<OrderDetailEntity>> GetByOrderIdsAsync(List<int> ids);
    Task<List<OrderDetailEntity>> GetByOrderIdAsync(int id);
    // Task<List<OrderDetailEntity>> GetOrderIdsByProductIdsAsync(List<int> productIds);
    Task<List<OrderSoldDetailDto>> GetOrderSoldDetailsBySellerIdAsync(int sellerId, int status = 3, DateTime? date = null);
}