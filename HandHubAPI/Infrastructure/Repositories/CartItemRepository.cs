using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;

namespace HandHubAPI.Infrastructure.Repositories;

public class CartItemRepository : BaseRepository<CartItemEntity>, ICartItemRepository
{
    public CartItemRepository(HandHubDbContext context) : base(context)
    {
    }
}