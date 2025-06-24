using System.Net;
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

    public class GetAllSoldOrdersRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int CurrentUserId { get; set; } = 0;
        public DateTime? date { get; set; }
        public int Status { get; set; } = 3; // 3 means all status
    }

    public class SearchOrderByCustomerNameRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int CustomerId { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
    }

    public class CancelOrderRequest
    {
        public int OrderId { get; set; }
        public string Reason { get; set; } = string.Empty;
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

    [HttpGet("get-all-orders")]
    public async Task<IActionResult> GetAllOrders([FromQuery] GetAllOrdersRequest request)
    {
        try
        {
            var orders = await _orderService.GetAllOrdersAsync(request.PageNumber, request.PageSize, request.CustomerId);
            return PaginatedResponse(
                orders.Items,
                orders.PageNumber,
                orders.PageSize,
                orders.TotalItems,
                "Orders retrieved successfully"
            );
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to retrieve products", HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("get-order-detail-by-id")]
    public async Task<IActionResult> GetOrderDetailById([FromQuery] int orderId)
    {
        try
        {
            var orderDetail = await _orderService.GetOrderDetailByIdAsync(orderId);
            if (orderDetail == null)
            {
                return ErrorResponse("Order detail not found", HttpStatusCode.NotFound);
            }
            return CommonResponse(orderDetail, "Order detail retrieved successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to retrieve order detail", HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("get-all-sold-orders")]
    public async Task<IActionResult> GetAllSoldOrders([FromQuery] GetAllSoldOrdersRequest request)
    {
        try
        {
            var orders = await _orderService.GetAllSoldOrdersByUserIdAsync(
                request.CurrentUserId,
                request.PageNumber,
                request.PageSize,
                request.date,
                request.Status
            );
            return PaginatedResponse(
                orders.Items,
                orders.PageNumber,
                orders.PageSize,
                orders.TotalItems,
                "Sold orders retrieved successfully"
            );
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to retrieve sold orders", HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPost("cancel-order")]
    public async Task<IActionResult> CancelOrder([FromBody] CancelOrderRequest request)
    {
        try
        {
            var result = await _orderService.CancelOrderAsync(request.OrderId, request.Reason);
            if (result == null)
            {
                return ErrorResponse("Order not found or cannot be cancelled", HttpStatusCode.BadRequest);
            }
            return CommonResponse(result, "Order cancelled successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to cancel order", HttpStatusCode.InternalServerError, ex);
        }
    }
}