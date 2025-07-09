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

    public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Order.AsQueryable();

        query = query.Where(o => o.Status == 2);
        if (startDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value <= endDate.Value);
        }

        return await query.SumAsync(o => o.TotalMoney);
    }

    public async Task<List<UserEntity>> GetUsersWithSoldOrdersInDateRange(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.User
            .Join(_context.Product, u => u.Id, p => p.SellerId, (u, p) => new { User = u, Product = p })
            .Join(_context.OrderDetail, up => up.Product.Id, od => od.ProductId, (up, od) => new { up.User, up.Product, OrderDetail = od })
            .Join(_context.Order, upod => upod.OrderDetail.OrderId, o => o.Id, (upod, o) => new { upod.User, Order = o })
            .Where(result => result.Order.Status == 2);

        if (startDate.HasValue)
        {
            query = query.Where(result => result.Order.CreatedAt.HasValue && result.Order.CreatedAt.Value >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(result => result.Order.CreatedAt.HasValue && result.Order.CreatedAt.Value <= endDate.Value);
        }

        return await query
            .Select(result => result.User)
            .Distinct()
            .ToListAsync();
    }

    public async Task<int> GetTotalOrdersByUser(int userId, DateTime? startDate, DateTime? endDate)
    {
        var result = await _context.User
            .Where(u => u.Id == userId)
            .Join(_context.Product, u => u.Id, p => p.SellerId, (u, p) => new { User = u, Product = p })
            .Join(_context.OrderDetail, up => up.Product.Id, od => od.ProductId, (up, od) => new { up.User, up.Product, OrderDetail = od })
            .Join(_context.Order, upod => upod.OrderDetail.OrderId, o => o.Id, (upod, o) => new { upod.User, Order = o })
            .Where(result => result.Order.Status == 2)
            .Where(result => !startDate.HasValue || (result.Order.CreatedAt.HasValue && result.Order.CreatedAt.Value >= startDate.Value))
            .Where(result => !endDate.HasValue || (result.Order.CreatedAt.HasValue && result.Order.CreatedAt.Value <= endDate.Value))
            .GroupBy(result => result.User.Id)
            .Select(group => group.Select(x => x.Order.Id).Distinct().Count())
            .FirstOrDefaultAsync();

        return result;
    }

    public async Task<decimal> GetTotalRevenueByUser(int userId, DateTime? startDate, DateTime? endDate)
    {
        var result = await (from u in _context.User
                            join p in _context.Product on u.Id equals p.SellerId
                            join od in _context.OrderDetail on p.Id equals od.ProductId
                            join o in _context.Order on od.OrderId equals o.Id
                            where o.Status == 2 && u.Id == userId
                            where !startDate.HasValue || (o.CreatedAt.HasValue && o.CreatedAt.Value >= startDate.Value)
                            where !endDate.HasValue || (o.CreatedAt.HasValue && o.CreatedAt.Value <= endDate.Value)
                            group o by u.Id into g
                            select g.Sum(x => x.TotalMoney)).FirstOrDefaultAsync();

        return result;
    }

    public async Task<decimal> GetTotalRevenueByWeekInMonth(int month, int year, int week)
    {
        var firstDayOfMonth = new DateTime(year, month, 1);
        var startOfWeek = firstDayOfMonth.AddDays((week - 1) * 7);
        var endOfWeek = startOfWeek.AddDays(6);

        // Ensure we don't go beyond the month boundaries
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
        if (endOfWeek > lastDayOfMonth)
        {
            endOfWeek = lastDayOfMonth;
        }

        var result = await (from o in _context.Order
                            where o.Status == 2
                            where o.OrderDate >= startOfWeek && o.OrderDate <= endOfWeek
                            select o.TotalMoney).SumAsync();

        return result;
    }
}