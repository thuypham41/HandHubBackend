using HandHubAPI.Application.DTOs;
using HandHubAPI.Application.Features.Interfaces;
using HandHubAPI.Controllers;
using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;
using HandHubAPI.Requests;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using static HandHubAPI.Controllers.CartController;

namespace HandHubAPI.Application.Features.Implements;

public class CartService : ICartService
{
    private readonly ILogger<CartService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public CartService(
        ILogger<CartService> logger,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<CartDto> CreateCartAsync(CartController.CreateCartRequest request)
    {
        // Validate request
        if (request == null || request.UserId <= 0)
            throw new ArgumentException("Invalid cart creation request.");

        // Check if user exists
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        // Create new cart entity
        var cart = new CartEntity
        {
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        // Add cart to repository
        await _unitOfWork.CartRepository.AddAsync(cart);
        await _unitOfWork.CommitAsync();

        // Map to DTO
        var cartDto = new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            CreatedAt = cart.CreatedAt,
            UpdatedAt = cart.UpdatedAt,
            Items = []
        };

        return cartDto;
    }

    public async Task<int> GetByUserIdAsync(int userId)
    {
        // Retrieve the cart entity by user ID
        var cart = await _unitOfWork.CartRepository.GetByUserIdAsync(userId);
        if (cart == null)
            return 0;

        return cart.Id;
    }

    public async Task<CartDto> AddItemToCartAsync(CartController.AddCartItemRequest request)
    {
        // Validate request
        if (request == null || request.UserId <= 0 || request.ProductId <= 0 || request.Quantity <= 0)
            throw new ArgumentException("Invalid add item to cart request.");

        // Retrieve the cart for the user
        var cart = await _unitOfWork.CartRepository.GetByUserIdAsync(request.UserId);
        if (cart == null)
            throw new InvalidOperationException("Cart not found for user.");

        // Check if item already exists in cart
        var cartItem = await _unitOfWork.CartItemRepository
            .GetByCartAndProductIdAsync(cart.Id, request.ProductId);
        if (cartItem != null)
        {
            throw new InvalidOperationException("This product is already in the cart.");
        }
        else
        {
            var newItem = new CartItemEntity
            {
                CartId = cart.Id,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Price = request.Price,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.CartItemRepository.AddAsync(newItem);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        cart.TotalPrice += request.Price * request.Quantity;
        _unitOfWork.CartRepository.Update(cart);
        await _unitOfWork.CommitAsync();

        // Retrieve cart items from repository
        var cartItems = await _unitOfWork.CartItemRepository.GetByCartIdAsync(cart.Id, request.UserId);

        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            CreatedAt = cart.CreatedAt,
            UpdatedAt = cart.UpdatedAt,
            Items = [.. cartItems.Select(item => new CartItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = request.Price,
            })]
        };
    }

    public async Task<List<CartItemDto>> GetCartItemsAsync(int userId)
    {
        var cart = await _unitOfWork.CartRepository.GetByUserIdAsync(userId);
        if (cart == null)
            return new List<CartItemDto>();

        var cartItems = await _unitOfWork.CartItemRepository.GetByCartIdAsync(cart.Id, userId);
        return [.. cartItems.Select(item => new CartItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            Price = item.Price
        })];
    }

    public async Task<bool> RemoveItemFromCartAsync(int userId, int productId)
    {
        var cart = await _unitOfWork.CartRepository.GetByUserIdAsync(userId);
        if (cart == null)
            return false;

        var cartItem = await _unitOfWork.CartItemRepository.GetByCartAndProductIdAsync(cart.Id, productId);
        if (cartItem == null)
            return false;

        await _unitOfWork.CartItemRepository.Delete(cartItem.Id);
        cart.UpdatedAt = DateTime.UtcNow;
        cart.TotalPrice -= cartItem.Price * cartItem.Quantity;
        _unitOfWork.CartRepository.Update(cart);
        await _unitOfWork.CommitAsync();
        return true;
    }
}