using HandHubAPI.Application.DTOs;
using HandHubAPI.Controllers;
using HandHubAPI.Domain.Entities;
using HandHubAPI.Requests;
using static HandHubAPI.Controllers.CartController;

namespace HandHubAPI.Application.Features.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> CreateCartAsync(CreateCartRequest request);
        Task<int> GetByUserIdAsync(int userId);
        Task<CartItemDto> AddItemToCartAsync(AddCartItemRequest request);
        Task<List<CartItemDto>> GetCartItemsAsync(int cartId);
        Task<bool> RemoveItemFromCartAsync(int cartId, int productId);
        Task<bool> ClearAllCartbyUserIdAsync(int userId);
    }
}