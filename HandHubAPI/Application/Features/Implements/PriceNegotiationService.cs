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
            return null;
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
}