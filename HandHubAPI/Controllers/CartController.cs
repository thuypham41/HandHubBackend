using System.Net;
using HandHubAPI.Application.DTOs;
using HandHubAPI.Application.Features.Interfaces;
using HandHubAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace HandHubAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController : BaseController<CartController>
{
    private readonly ICartService _cartService;
    public CartController(
        ICartService cartService,
        ILogger<CartController> logger) : base(logger)
    {
        _cartService = cartService;
    }

    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
    }

    public class CartDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = [];
        public decimal TotalPrice { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class CreateCartRequest
    {
        public int UserId { get; set; }
    }

    public class UpdateCartRequest
    {
        public List<CartItemDto> Items { get; set; }
    }

    public class AddCartItemRequest
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    [HttpPost("create-cart")]
    public async Task<IActionResult> CreateCart([FromBody] CreateCartRequest request)
    {
        try
        {
            var cart = await _cartService.CreateCartAsync(request);
            return CommonResponse(cart, "Cart created successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to create cart", HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPost("add-item")]
    public async Task<IActionResult> AddItemToCart([FromBody] AddCartItemRequest request)
    {
        try
        {
            var cartId = await _cartService.GetByUserIdAsync(request.UserId);
            if (cartId == 0)
            {
                var cart = await _cartService.CreateCartAsync(new CreateCartRequest { UserId = request.UserId });
                cartId = cart.Id;
            }

            request.CartId = cartId;
            var result = await _cartService.AddItemToCartAsync(request);
            return CommonResponse(result, "Item added to cart successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Item is exist in cart", HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("get-items")]
    public async Task<IActionResult> GetAllItems(int userId)
    {
        try
        {
            var items = await _cartService.GetCartItemsAsync(userId);
            return CommonResponse(items, "Cart items retrieved successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to get cart items", HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpDelete("remove-item")]
    public async Task<IActionResult> RemoveItem([FromQuery] int userId, [FromQuery] int productId)
    {
        try
        {
            var result = await _cartService.RemoveItemFromCartAsync(userId, productId);
            if (!result)
                return NotFound("Item not found in cart.");

            return CommonResponse(result, "Item removed from cart successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to remove item from cart", HttpStatusCode.InternalServerError, ex);
        }
    }
}