using HandHubAPI.Application.DTOs;
using HandHubAPI.Controllers;
using HandHubAPI.Domain.Entities;
using HandHubAPI.Requests;
using static HandHubAPI.Controllers.PriceNegotiationController;

namespace HandHubAPI.Application.Features.Interfaces
{
    public interface IPriceNegotiationService
    {
        Task<PriceNegotiationEntity?> AddPriceNegotiationAsync(AddPriceNegotiationRequest request);
        Task<NegotiationMessageEntity?> AddNegotiationMessageAsync(AddNegotiationMessageRequest request);
        Task<PriceNegotiationEntity?> GetByIdAsync(int priceNegotiationId);
        Task<ProductEntity?> GetProductByIdAsync(int productId);
        Task<UserEntity?> GetUserByIdAsync(int userId);
        Task<IEnumerable<NegotiationMessageEntity>> GetAllMessagesAsync(int priceNegotiationId);
    }
}