using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;

namespace HandHubAPI.Infrastructure.Repositories;

public class PriceNegotiationRepository : BaseRepository<PriceNegotiationEntity>, IPriceNegotiationRepository
{
    public PriceNegotiationRepository(HandHubDbContext context) : base(context)
    {
    }
}