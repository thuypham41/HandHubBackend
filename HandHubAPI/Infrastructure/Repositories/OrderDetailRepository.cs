using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HandHubAPI.Infrastructure.Repositories;

public class OrderDetailRepository : BaseRepository<OrderDetailEntity>, IOrderDetailRepository
{
    public OrderDetailRepository(HandHubDbContext context) : base(context)
    {
    }
    public async Task<List<OrderDetailEntity>> GetOrderIdsByProductIdsAsync(List<int> productIds)
    {
        return await _context.Set<OrderDetailEntity>()
            .Where(od => productIds.Contains(od.ProductId))
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<OrderDetailEntity>> GetByOrderIdAsync(int id)
    {
        return await _context.Set<OrderDetailEntity>()
            .Where(od => od.OrderId == id)
            .ToListAsync();
    }

    public async Task<List<OrderDetailEntity>> GetByOrderIdsAsync(List<int> ids)
    {
        return await _context.Set<OrderDetailEntity>()
            .Where(od => ids.Contains(od.OrderId))
            .ToListAsync();
    }
}