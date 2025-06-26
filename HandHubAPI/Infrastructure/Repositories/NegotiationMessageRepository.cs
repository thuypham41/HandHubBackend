using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HandHubAPI.Infrastructure.Repositories;

public class NegotiationMessageRepository : BaseRepository<NegotiationMessageEntity>, INegotiationMessageRepository
{
    public NegotiationMessageRepository(HandHubDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<NegotiationMessageEntity>> GetAllMessagesByNegotiationIdAsync(int negotiationId)
    {
        return await _context.Set<NegotiationMessageEntity>()
            .Where(m => m.PriceNegotiationId == negotiationId && !m.IsDeleted)
            .ToListAsync();
    }
}