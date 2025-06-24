using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HandHubAPI.Infrastructure.Repositories;

public class PriceNegotiationRepository : BaseRepository<PriceNegotiationEntity>, IPriceNegotiationRepository
{
    public PriceNegotiationRepository(HandHubDbContext context) : base(context)
    {
    }

    public async Task<PriceNegotiationEntity?> GetByProductAndBuyerAsync(int productId, int buyerId)
    {
        return await _context.PriceNegotiation
            .FirstOrDefaultAsync(pn => pn.ProductId == productId && pn.BuyerId == buyerId);
    }
}