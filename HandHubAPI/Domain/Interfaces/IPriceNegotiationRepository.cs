namespace HandHubAPI.Domain.Repositories;

using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;

public interface IPriceNegotiationRepository : IBaseRepository<PriceNegotiationEntity>
{
    Task<PriceNegotiationEntity?> GetByProductAndBuyerAsync(int productId, int buyerId);
}