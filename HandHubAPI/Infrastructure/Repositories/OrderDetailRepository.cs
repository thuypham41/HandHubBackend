using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;

namespace HandHubAPI.Infrastructure.Repositories;

public class OrderDetailRepository : BaseRepository<OrderDetailEntity>, IOrderDetailRepository
{
    public OrderDetailRepository(HandHubDbContext context) : base(context)
    {
    }
}