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
    // [HttpGet("{userId}")]
    // public async Task<ActionResult<CartDto>> GetCart(int userId)
    // {
    //     var cart = await _cartService.GetCartByUserIdAsync(userId);
    //     if (cart == null)
    //         return NotFound();
    //     return Ok(cart);
    // }

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
            return ErrorResponse("Failed to add item to cart", HttpStatusCode.InternalServerError, ex);
        }
    }
}