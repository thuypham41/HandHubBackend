using HandHubAPI.Application.DTOs;
using HandHubAPI.Controllers;
using HandHubAPI.Domain.Entities;
using HandHubAPI.Requests;
using static HandHubAPI.Controllers.CartController;

namespace HandHubAPI.Application.Features.Interfaces
{
    public interface ICartService
    {
        // Task<CartDto> GetCartByUserIdAsync(int userId);
        Task<CartDto> CreateCartAsync(CreateCartRequest request);
        Task<int> GetByUserIdAsync(int userId);
        // Task<CartDto> UpdateCartAsync(int userId, UpdateCartRequest request);
        Task<CartDto> AddItemToCartAsync(AddCartItemRequest request);
        // Task<CartDto> RemoveItemFromCartAsync(int userId, int itemId);
        // Task<decimal> CalculateTotalPriceAsync(int userId);
        // Task<bool> ClearCartAsync(int userId);
    }
}