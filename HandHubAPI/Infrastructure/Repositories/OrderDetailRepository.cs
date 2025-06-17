using HandHubAPI.Application.DTOs;
using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HandHubAPI.Infrastructure.Repositories;

public class OrderDetailRepository : BaseRepository<OrderDetailEntity>, IOrderDetailRepository
{
    public OrderDetailRepository(HandHubDbContext context) : base(context)
    {
    }
    public async Task<List<OrderSoldDetailDto>> GetOrderSoldDetailsBySellerIdAsync(int sellerId, int status = 3, DateTime? date = null)
    {
        var query = from p in _context.Product
                    join od in _context.OrderDetail on p.Id equals od.ProductId
                    join o in _context.Order on od.OrderId equals o.Id
                    where o.BuyerId != sellerId && p.SellerId == sellerId
                    select new
                    {
                        OrderDetailId = od.Id,
                        OrderId = od.OrderId,
                        Product = new ProductDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            Price = p.Price,
                            CategoryId = p.CategoryId,
                            ImageUrl = p.ImageUrl
                        },
                        Quantity = od.Num,
                        Price = od.Price,
                        TotalMoney = od.Num * od.Price,
                        Status = o.Status,
                        BuyerId = o.BuyerId,
                        OrderDate = o.CreatedAt
                    };

        if (status != 3)
        {
            query = query.Where(x => x.Status == status);
        }

        if (date.HasValue)
        {
            var selectedDate = date.Value.Date;
            query = query.Where(x => x.OrderDate.HasValue && x.OrderDate.Value.Date >= selectedDate);
        }

        var orderDetails = await query.ToListAsync();

        var buyerIds = orderDetails.Select(x => x.BuyerId).Distinct().ToList();
        var buyers = await _context.User
            .Where(u => buyerIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FullName);

        var result = orderDetails.Select(x => new OrderSoldDetailDto
        {
            OrderDetailId = x.OrderDetailId,
            OrderId = x.OrderId,
            Product = x.Product,
            Quantity = x.Quantity,
            Price = x.Price,
            TotalMoney = x.TotalMoney,
            Status = x.Status,
            BuyerName = buyers.ContainsKey(x.BuyerId) ? buyers[x.BuyerId] : "Unknown Buyer"
        }).ToList();

        return result;
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