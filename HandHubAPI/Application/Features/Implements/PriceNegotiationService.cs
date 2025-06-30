using HandHubAPI.Application.DTOs;
using HandHubAPI.Application.Features.Interfaces;
using HandHubAPI.Controllers;
using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;
using HandHubAPI.Requests;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using static HandHubAPI.Controllers.PriceNegotiationController;

namespace HandHubAPI.Application.Features.Implements;

public class PriceNegotiationService : IPriceNegotiationService
{
    private readonly ILogger<PriceNegotiationService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public PriceNegotiationService(
        ILogger<PriceNegotiationService> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<PriceNegotiationEntity?> AddPriceNegotiationAsync(AddPriceNegotiationRequest request)
    {
        // Check if price negotiation already exists for this product and buyer
        var existingNegotiation = await _unitOfWork.PriceNegotiationRepository
            .GetByProductAndBuyerAsync(request.ProductId, request.BuyerId);

        if (existingNegotiation != null)
        {
            // Update the existing negotiation's offer price
            existingNegotiation.OfferPrice = request.OfferPrice;
            existingNegotiation.SellerResponse = request?.SellerResponse ?? string.Empty;

            _unitOfWork.PriceNegotiationRepository.Update(existingNegotiation);
            await _unitOfWork.CommitAsync();

            return existingNegotiation;
        }

        var priceNegotiation = new PriceNegotiationEntity
        {
            ProductId = request.ProductId,
            OfferPrice = request.OfferPrice,
            BuyerId = request.BuyerId,
            SellerResponse = request?.SellerResponse ?? string.Empty,
        };

        await _unitOfWork.PriceNegotiationRepository.AddAsync(priceNegotiation);
        await _unitOfWork.CommitAsync();

        return priceNegotiation;
    }

    public async Task<ProductEntity?> GetProductByIdAsync(int productId)
    {
        return await _unitOfWork.ProductRepository.GetByIdAsync(productId);
    }

    public async Task<UserEntity?> GetUserByIdAsync(int userId)
    {
        return await _unitOfWork.UserRepository.GetByIdAsync(userId);
    }

    public async Task<PriceNegotiationEntity?> GetByIdAsync(int id)
    {
        return await _unitOfWork.PriceNegotiationRepository.GetByIdAsync(id);
    }

    public async Task<NegotiationMessageEntity> AddNegotiationMessageAsync(AddNegotiationMessageRequest request)
    {
        try
        {
            // Get the existing price negotiation
            var negotiation = await _unitOfWork.PriceNegotiationRepository.GetByIdAsync(request.PriceNegotiationId);
            if (negotiation == null)
            {
                _logger.LogWarning("Price negotiation with ID {NegotiationId} not found", request.PriceNegotiationId);
                return null;
            }

            // Create new negotiation message
            var message = new NegotiationMessageEntity
            {
                PriceNegotiationId = request.PriceNegotiationId,
                ReceivierId = request.ReceiverId,
                SenderId = request.SenderId,
                MessageContent = request.MessageContent,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.NegotiationMessageRepository.AddAsync(message);
            await _unitOfWork.CommitAsync();

            return message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding negotiation message for negotiation {NegotiationId}", request.PriceNegotiationId);
            return null;
        }
    }

    public async Task<IEnumerable<NegotiationMessageEntity>> GetAllMessagesAsync(int priceNegotiationId)
    {
        try
        {
            return await _unitOfWork.NegotiationMessageRepository.GetAllMessagesByNegotiationIdAsync(priceNegotiationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving messages for negotiation {NegotiationId}", priceNegotiationId);
            return Enumerable.Empty<NegotiationMessageEntity>();
        }
    }
}