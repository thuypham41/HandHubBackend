using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;

namespace HandHubAPI.Infrastructure.Repositories;

public class CartRepository : BaseRepository<CartEntity>, ICartRepository
{
    public CartRepository(HandHubDbContext context) : base(context)
    {
    }
}