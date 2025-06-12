using System.Net;
using HandHubAPI.Application.DTOs;
using HandHubAPI.Application.Features.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace HandHubAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : BaseController<OrderController>
{
    private readonly IOrderService _orderService;
    public OrderController(IOrderService orderService, ILogger<OrderController> logger) : base(logger)
    {
        _orderService = orderService;
    }

    // Request DTOs
    public class GetOrderByIdRequest
    {
        public int Id { get; set; }
    }

    public class DeleteOrderRequest
    {
        public int Id { get; set; }
    }

    public class GetAllOrdersRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int CustomerId { get; set; } = 0;
    }

    public class SearchOrderByCustomerNameRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int CustomerId { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
    }

    // [HttpGet("get-order-by-id")]
    // public async Task<IActionResult> GetOrderById([FromQuery] GetOrderByIdRequest request)
    // {
    //     try
    //     {
    //         var order = await _orderService.GetOrderByIdAsync(request.Id);
    //         if (order == null)
    //         {
    //             return ErrorResponse("Order not found", HttpStatusCode.NotFound);
    //         }
    //         return CommonResponse(order, "Order retrieved successfully");
    //     }
    //     catch (Exception ex)
    //     {
    //         return ErrorResponse("Failed to retrieve order", HttpStatusCode.InternalServerError, ex);
    //     }
    // }

    // [HttpPost("create-order")]
    // public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    // {
    //     try
    //     {
    //         var order = await _orderService.CreateOrderAsync(request);
    //         return CommonResponse(order, "Order created successfully");
    //     }
    //     catch (Exception ex)
    //     {
    //         return ErrorResponse("Failed to create order", HttpStatusCode.InternalServerError, ex);
    //     }
    // }

    // [HttpPut("update-order")]
    // public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderRequest request)
    // {
    //     try
    //     {
    //         var order = await _orderService.UpdateOrderAsync(request.Id, request);
    //         if (order == null)
    //         {
    //             return ErrorResponse("Order not found", HttpStatusCode.NotFound);
    //         }
    //         return CommonResponse(order, "Order updated successfully");
    //     }
    //     catch (Exception ex)
    //     {
    //         return ErrorResponse("Failed to update order", HttpStatusCode.InternalServerError, ex);
    //     }
    // }

    // [HttpDelete("delete-order/{id}")]
    // public async Task<IActionResult> DeleteOrder([FromRoute] DeleteOrderRequest request)
    // {
    //     try
    //     {
    //         var result = await _orderService.DeleteOrderAsync(request.Id);
    //         if (!result)
    //         {
    //             return ErrorResponse("Order not found", HttpStatusCode.NotFound);
    //         }
    //         return CommonResponse(result, "Order deleted successfully");
    //     }
    //     catch (Exception ex)
    //     {
    //         return ErrorResponse("Failed to delete order", HttpStatusCode.InternalServerError, ex);
    //     }
    // }

    // [HttpGet("get-all-orders")]
    // public async Task<IActionResult> GetAllOrders([FromQuery] GetAllOrdersRequest request)
    // {
    //     try
    //     {
    //         var orders = await _orderService.GetAllOrdersAsync(request.PageNumber, request.PageSize, request.CustomerId);
    //         return PaginatedResponse(
    //             orders.Items,
    //             orders.PageNumber,
    //             orders.PageSize,
    //             orders.TotalItems,
    //             "Orders retrieved successfully"
    //         );
    //     }
    //     catch (Exception ex)
    //     {
    //         return ErrorResponse("Failed to retrieve products", HttpStatusCode.InternalServerError, ex);
    //     }
    // }

    // [HttpGet("search-by-name")]
    // public async Task<IActionResult> SearchOrderByName([FromQuery] SearchOrderByNameRequest request)
    // {
    //     try
    //     {
    //         var orders = await _orderService.GetAllOrdersAsync(
    //             request.PageNumber,
    //             request.PageSize,
    //             customerId: request.CustomerId,
    //             searchTerm: request.Name
    //         );
    //         return PaginatedResponse(
    //             orders.Items,
    //             orders.PageNumber,
    //             orders.PageSize,
    //             orders.TotalItems,
    //             "Orders retrieved successfully"
    //         );
    //     }
    //     catch (Exception ex)
    //     {
    //         return ErrorResponse("Failed to search orders", HttpStatusCode.InternalServerError, ex);
    //     }
    // }
}