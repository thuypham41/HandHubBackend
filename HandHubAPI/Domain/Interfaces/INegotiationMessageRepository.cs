namespace HandHubAPI.Domain.Repositories;

using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;

public interface INegotiationMessageRepository : IBaseRepository<NegotiationMessageEntity>
{
    Task<IEnumerable<NegotiationMessageEntity>> GetAllMessagesByNegotiationIdAsync(int priceNegotiationId);
}