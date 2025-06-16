using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HandHubAPI.Infrastructure.Repositories;

public class OrderRepository : BaseRepository<OrderEntity>, IOrderRepository
{
    public OrderRepository(HandHubDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<int>> GetPurchasedCategoryIdsByUserAsync(int userId)
    {
        var orders = await _context.Order
            .Where(o => o.BuyerId == userId)
            .Select(o => o.Id)
            .ToListAsync();

        var products = await _context.OrderDetail
            .Where(od => orders.Contains(od.OrderId))
            .Select(od => od.ProductId)
            .Distinct()
            .ToListAsync();

        var categoryIds = await _context.Product
            .Where(p => products.Contains(p.Id))
            .Select(c => c.CategoryId)
            .Distinct().ToListAsync();

        return categoryIds;
    }
}