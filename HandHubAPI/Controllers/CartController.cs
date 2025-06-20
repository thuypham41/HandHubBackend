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
    }

    public class CartDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItemDto> Items { get; set; }
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
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
    [HttpGet("{userId}")]
    public async Task<ActionResult<CartDto>> GetCart(int userId)
    {
        var cart = await _cartService.GetCartByUserIdAsync(userId);
        if (cart == null)
            return NotFound();
        return Ok(cart);
    }

    [HttpPost]
    public async Task<ActionResult<CartDto>> CreateCart([FromBody] CreateCartRequest request)
    {
        var cart = await _cartService.CreateCartAsync(request);
        return CreatedAtAction(nameof(GetCart), new { userId = cart.UserId }, cart);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CartDto>> UpdateCart(int id, [FromBody] UpdateCartRequest request)
    {
        var cart = await _cartService.UpdateCartAsync(id, request);
        if (cart == null)
            return NotFound();
        return Ok(cart);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCart(int id)
    {
        var result = await _cartService.DeleteCartAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpPost("{cartId}/items")]
    public async Task<ActionResult> AddItemToCart(int cartId, [FromBody] AddCartItemRequest request)
    {
        var result = await _cartService.AddItemToCartAsync(cartId, request);
        if (!result)
            return NotFound();
        return Ok();
    }

    [HttpDelete("{cartId}/items/{itemId}")]
    public async Task<ActionResult> RemoveItemFromCart(int cartId, int itemId)
    {
        var result = await _cartService.RemoveItemFromCartAsync(cartId, itemId);
        if (!result)
            return NotFound();
        return NoContent();
    }
}