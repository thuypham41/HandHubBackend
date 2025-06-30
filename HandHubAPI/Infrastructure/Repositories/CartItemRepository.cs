using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HandHubAPI.Infrastructure.Repositories;

public class CartItemRepository : BaseRepository<CartItemEntity>, ICartItemRepository
{
    public CartItemRepository(HandHubDbContext context) : base(context)
    {
    }

    public async Task<CartItemEntity?> GetByCartAndProductIdAsync(int cartId, int productId)
    {
        return await _context.Set<CartItemEntity>()
            .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
    }

    public async Task<bool> ExistsByCartAndProductIdAsync(int cartId, int productId)
    {
        return await _context.Set<CartItemEntity>()
            .AnyAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
    }
    public async Task<IEnumerable<CartItemEntity>> GetByCartIdAsync(int cartId, int userId = 0)
    {
        return await _context.Set<CartItemEntity>()
            .Where(ci => ci.CartId == cartId && !ci.IsDeleted)
            .ToListAsync();
    }
}