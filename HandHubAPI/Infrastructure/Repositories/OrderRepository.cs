using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;

namespace HandHubAPI.Infrastructure.Repositories;

public class OrderRepository : BaseRepository<OrderEntity>, IOrderRepository
{
    public OrderRepository(HandHubDbContext context) : base(context)
    {
    }
}