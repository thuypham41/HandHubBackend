using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HandHubAPI.Infrastructure.Repositories;

public class CartRepository : BaseRepository<CartEntity>, ICartRepository
{
    public CartRepository(HandHubDbContext context) : base(context)
    {
    }

    public async Task<CartEntity?> GetByUserIdAsync(int userId)
    {
        return await _context.Set<CartEntity>().FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<bool> ExistsByUserIdAsync(int userId)
    {
        return await _context.Set<CartEntity>().AnyAsync(c => c.UserId == userId);
    }
}