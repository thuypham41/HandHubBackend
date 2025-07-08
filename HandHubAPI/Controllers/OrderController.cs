using System.Net;
using HandHubAPI.Application.Features.Interfaces;
using HandHubAPI.Payment.Enums;
using HandHubAPI.Payment.Interfaces;
using HandHubAPI.Payment.Models;
using HandHubAPI.Payment.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace HandHubAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : BaseController<OrderController>
{
    private readonly IOrderService _orderService;
    private readonly IVnpay _vnpay;
    public OrderController(IOrderService orderService, ILogger<OrderController> logger, IConfiguration configuration) : base(logger)
    {
        _orderService = orderService;
        _vnpay = new Vnpay();

        var tmnCode = configuration["Vnpay:TmnCode"];
        var hashSecret = configuration["Vnpay:HashSecret"];
        var baseUrl = configuration["Vnpay:BaseUrl"];
        var returnUrl = configuration["Vnpay:ReturnUrl"];

        if (string.IsNullOrEmpty(tmnCode) || string.IsNullOrEmpty(hashSecret) || string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(returnUrl))
        {
            throw new InvalidOperationException("One or more VNPAY configuration values are missing.");
        }

        _vnpay.Initialize(
            tmnCode,
            hashSecret,
            baseUrl,
            returnUrl
        );
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

    public class CreateOrderRequest
    {
        public int CustomerId { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
        public int TotalMoney { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
    }

    public class OrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
    }

    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(request);
            return CommonResponse(order, "Order created successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to create order", HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("CreatePaymentUrl")]
    public ActionResult<string> CreatePaymentUrl(double moneyToPay, string description)
    {
        try
        {
            var ipAddress = NetworkHelper.GetIpAddress(HttpContext); // Lấy địa chỉ IP của thiết bị thực hiện giao dịch

            var request = new PaymentRequest
            {
                PaymentId = DateTime.Now.Ticks,
                Money = moneyToPay,
                Description = description,
                IpAddress = ipAddress,
                BankCode = BankCode.ANY, // Tùy chọn. Mặc định là tất cả phương thức giao dịch
                CreatedDate = DateTime.Now, // Tùy chọn. Mặc định là thời điểm hiện tại
                Currency = Currency.VND, // Tùy chọn. Mặc định là VND (Việt Nam đồng)
                Language = DisplayLanguage.Vietnamese // Tùy chọn. Mặc định là tiếng Việt
            };
            System.Console.WriteLine("diendk: " + JsonConvert.SerializeObject(request));
            var paymentUrl = _vnpay.GetPaymentUrl(request);

            return Created(paymentUrl, paymentUrl);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("IpnAction")]
    public IActionResult IpnAction()
    {
        if (Request.QueryString.HasValue)
        {
            try
            {
                var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                if (paymentResult.IsSuccess)
                {
                    // Thực hiện hành động nếu thanh toán thành công tại đây. Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu.
                    return Ok();
                }

                // Thực hiện hành động nếu thanh toán thất bại tại đây. Ví dụ: Hủy đơn hàng.
                return BadRequest("Thanh toán thất bại");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        return NotFound("Không tìm thấy thông tin thanh toán.");
    }

    [HttpGet("Callback")]
    public ActionResult<string> Callback()
    {
        if (Request.QueryString.HasValue)
        {
            try
            {
                var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                var resultDescription = $"{paymentResult.PaymentResponse.Description}. {paymentResult.TransactionStatus.Description}.";

                if (paymentResult.IsSuccess)
                {
                    return Ok(resultDescription);
                }
                System.Console.WriteLine($"diendk: " + resultDescription);
                return BadRequest(resultDescription);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        return NotFound("Không tìm thấy thông tin thanh toán.");
    }
}