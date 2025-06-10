using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;

namespace HandHubAPI.Infrastructure.Repositories;

public class NegotiationMessageRepository : BaseRepository<NegotiationMessageEntity>, INegotiationMessageRepository
{
    public NegotiationMessageRepository(HandHubDbContext context) : base(context)
    {
    }
}