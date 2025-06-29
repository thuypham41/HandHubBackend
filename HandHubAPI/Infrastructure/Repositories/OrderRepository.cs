using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HandHubAPI.Infrastructure.Repositories;

public class OrderRepository : BaseRepository<OrderEntity>, IOrderRepository
{
    public OrderRepository(HandHubDbContext context) : base(context)
    {
    }
    public async Task<List<OrderEntity>> GetByIdsAsync(List<int> ids, int status = 3, DateTime? date = null, int currentUserId = 0)
    {
        var query = _context.Order.AsQueryable();

        if (ids != null && ids.Count > 0)
        {
            query = query.Where(o => ids.Contains(o.Id));
        }

        if (currentUserId > 0)
        {
            query = query.Where(o => o.BuyerId != currentUserId);
        }

        if (status != 3)
        {
            query = query.Where(o => o.Status == status);
        }

        if (date.HasValue)
        {
            var targetDate = date.Value.Date;
            query = query.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Date >= targetDate);
        }

        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
    }

    public async Task<PaginatedResponse<OrderEntity>> GetPaginatedAsync(int pageNumber, int pageSize, int customerId = 0, string? searchTerm = null)
    {
        var query = _context.Order.AsQueryable();

        if (customerId > 0)
        {
            query = query.Where(o => o.BuyerId == customerId);
        }

        // if (!string.IsNullOrWhiteSpace(searchTerm))
        // {
        //     query = query.Where(o => o.OrderNumber.Contains(searchTerm));
        // }

        var totalItems = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponse<OrderEntity>
        {
            Items = items,
            TotalItems = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
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