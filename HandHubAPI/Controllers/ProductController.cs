using System.Net;
using HandHubAPI.Application.DTOs;
using HandHubAPI.Application.Features.Interfaces;
using HandHubAPI.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
namespace HandHubAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : BaseController<ProductController>
{
    private readonly IProductService _productService;
    private const string MESSAGE_NOTIFICATION_TITLE = "Thông báo sản phẩm";
    private const string MESSAGE_NOTIFICATION = "Bạn đã nhận được một thông báo về sản phẩm";
    private readonly IChatHubService _chatHubService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public ProductController(
        IProductService productService,
        ILogger<ProductController> logger,
        IChatHubService chatHubService,
        Microsoft.AspNetCore.SignalR.IHubContext<NotificationHub> hubContext
    ) : base(logger)
    {
        _chatHubService = chatHubService;
        _productService = productService;
        _hubContext = hubContext;
    }

    // Request DTOs
    public class GetProductByIdRequest
    {
        public int Id { get; set; }
    }

    public class DeleteProductRequest
    {
        public int Id { get; set; }
    }

    public class GetAllProductsRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int CategoryId { get; set; } = 0;
    }

    public class GetAllProductsByStatus
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int CategoryId { get; set; } = 0;
        public int Status { get; set; } = 1; // Default to 1 (approved products)
    }

    public class SearchProductByNameRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int CategoryId { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
    }

    public class GetRecentProductsRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    [HttpGet("get-product-by-id")]
    public async Task<IActionResult> GetProductById([FromQuery] GetProductByIdRequest request)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(request.Id);
            if (product == null)
            {
                return ErrorResponse("Product not found", HttpStatusCode.NotFound);
            }
            return CommonResponse(product, "Product retrieved successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to retrieve product", HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPost("create-product")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            var product = await _productService.CreateProductAsync(request);
            return CommonResponse(product, "Product created successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to create product", HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPut("update-product")]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductRequest request)
    {
        try
        {
            var product = await _productService.UpdateProductAsync(request.Id, request);
            if (product == null)
            {
                return ErrorResponse("Product not found", HttpStatusCode.NotFound);
            }
            return CommonResponse(product, "Product updated successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to update product", HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpDelete("delete-product")]
    public async Task<IActionResult> DeleteProduct([FromQuery] DeleteProductRequest request)
    {
        try
        {
            var result = await _productService.DeleteProductAsync(request.Id);
            if (!result)
            {
                return ErrorResponse("Product not found", HttpStatusCode.NotFound);
            }
            return CommonResponse(result, "Product deleted successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to delete product", HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("get-all-products")]
    public async Task<IActionResult> GetAllProducts([FromQuery] GetAllProductsRequest request)
    {
        try
        {
            var products = await _productService.GetAllProductsAsync(request.PageNumber, request.PageSize, request.CategoryId);
            return PaginatedResponse(
                products.Items,
                products.PageNumber,
                products.PageSize,
                products.TotalItems,
                "Products retrieved successfully"
            );
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to retrieve products", HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("search-by-name")]
    public async Task<IActionResult> SearchProductByName([FromQuery] SearchProductByNameRequest request)
    {
        try
        {
            var products = await _productService.GetAllProductsAsync(
                request.PageNumber,
                request.PageSize,
                categoryId: request.CategoryId,
                1,
                searchTerm: request.Name
            );
            return PaginatedResponse(
                products.Items,
                products.PageNumber,
                products.PageSize,
                products.TotalItems,
                "Products retrieved successfully"
            );
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to search products", HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("get-products-by-status")]
    public async Task<IActionResult> GetAllProductByStatus([FromQuery] GetAllProductsByStatus request)
    {
        try
        {
            var products = await _productService.GetAllProductsAsync(
                request.PageNumber,
                request.PageSize,
                categoryId: request.CategoryId,
                status: request.Status
            );
            return PaginatedResponse(
                products.Items,
                products.PageNumber,
                products.PageSize,
                products.TotalItems,
                "Products retrieved successfully"
            );
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to search products", HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("get-recent-products")]
    public async Task<IActionResult> GetRecentProducts([FromQuery] GetRecentProductsRequest request)
    {
        try
        {
            var products = await _productService.GetRecentProducts(
                request.PageNumber,
                request.PageSize
            );
            return PaginatedResponse(
                products.Items,
                products.PageNumber,
                products.PageSize,
                products.TotalItems,
                "Recently created products retrieved successfully"
            );
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to retrieve recent products", HttpStatusCode.InternalServerError, ex);
        }
    }

    public class GetSubCategoryProductsRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SearchTerm { get; set; } = null;
        public int SubCategoryId { get; set; }
        public int CurrentUserId { get; set; } = 0; // Optional, for user-specific logic
    }

    [HttpGet("get-subcategory-products")]
    public async Task<IActionResult> GetSubCategoryProducts([FromQuery] GetSubCategoryProductsRequest request)
    {
        try
        {
            var products = await _productService.GetProductsBySubCategoryAsync(
                request.PageNumber,
                request.PageSize,
                request.SubCategoryId,
                request.CurrentUserId
            );
            return PaginatedResponse(
                products.Items,
                products.PageNumber,
                products.PageSize,
                products.TotalItems,
                "Products by subcategory retrieved successfully"
            );
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to retrieve subcategory products", HttpStatusCode.InternalServerError, ex);
        }
    }

    public class GetSuggestedProductsRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int UserId { get; set; }
    }

    [HttpGet("get-suggested-products")]
    public async Task<IActionResult> GetSuggestedProducts([FromQuery] GetSuggestedProductsRequest request)
    {
        try
        {
            var products = await _productService.GetSuggestedProductsByPurchasedCategoryAsync(
                request.PageNumber,
                request.PageSize,
                request.UserId
            );
            return PaginatedResponse(
                products.Items,
                products.PageNumber,
                products.PageSize,
                products.TotalItems,
                "Suggested products retrieved successfully"
            );
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to retrieve suggested products", HttpStatusCode.InternalServerError, ex);
        }
    }

    public class GetProductsBySellerWithoutOrderRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int SellerId { get; set; }
    }

    [HttpGet("get-products-by-seller-without-order")]
    public async Task<IActionResult> GetProductsBySellerWithoutOrder([FromQuery] GetProductsBySellerWithoutOrderRequest request)
    {
        try
        {
            var products = await _productService.GetProductsBySellerWithoutOrderAsync(
                request.PageNumber,
                request.PageSize,
                request.SellerId
            );
            return PaginatedResponse(
                products.Items,
                products.PageNumber,
                products.PageSize,
                products.TotalItems,
                "Products by seller without order retrieved successfully"
            );
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to retrieve products", HttpStatusCode.InternalServerError, ex);
        }
    }

    public class UpdateProductStatusRequest
    {
        public int AdminId { get; set; } // Admin or user performing the action
        public int ProductId { get; set; }
        public int Status { get; set; } // 1 = Approved, -1 = Rejected, etc.
    }

    [HttpPut("update-product-status")]
    public async Task<IActionResult> UpdateProductStatus([FromBody] UpdateProductStatusRequest request)
    {
        try
        {
            var updatedProduct = await _productService.UpdateProductStatusAsync(request.ProductId, request.Status);
            if (updatedProduct == null)
            {
                return ErrorResponse("Product not found", HttpStatusCode.NotFound);
            }
            string message = request.Status == 1 ? "Product approved successfully" :
                             request.Status == -1 ? "Product rejected successfully" :
                             "Product status updated successfully";

            // Send notification to seller if product is approved or rejected
            // You may need to inject IHubContext<NotificationHub> and IProductService if not already
            if (request.Status == 1 || request.Status == -1)
            {
                // Get sellerId from updatedProduct (assuming it has SellerId property)
                int sellerId = updatedProduct.SellerId;
                string notificationTitle = request.Status == 1 ? "Sản phẩm đã được duyệt" : "Sản phẩm bị từ chối";
                string notificationMessage = request.Status == 1
                    ? $"Sản phẩm \"{updatedProduct.Name}\" của bạn đã được duyệt."
                    : $"Sản phẩm \"{updatedProduct.Name}\" của bạn đã bị từ chối.";

                await SaveNotificationToUser(
                    updatedProduct.Id,
                    request.AdminId,
                    updatedProduct.SellerId,
                    notificationMessage,
                    "Đề xuất giá mới",
                    null,
                    request.ProductId);
                await _hubContext.Clients.User(sellerId.ToString()).SendAsync("ReceiveNotification", new
                {
                    SenderId = 0, // System/admin
                    ReceiverId = sellerId,
                    Message = notificationMessage,
                    Title = notificationTitle,
                    CreatedAt = DateTime.UtcNow,
                    ProductId = updatedProduct.Id,
                    Type = 3 // Custom type for product status notification
                });
            }

            return CommonResponse(updatedProduct, message);
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to update product status", HttpStatusCode.InternalServerError, ex);
        }
    }

    private async Task<NotificationDto> SaveNotificationToUser(
   int priceNegotiationId, int senderId, int reciverId, string message, string title, string? imageUrl, int productId)
    {
        var sendDatetime = DateTime.UtcNow;

        var notificationViewModel = new NotificationDto
        {
            SenderId = senderId,
            ReceiverId = reciverId,
            Messeage = message,
            CreatedAt = sendDatetime,
            UpdatedAt = sendDatetime,
            Title = title,
            Subtitle = MESSAGE_NOTIFICATION_TITLE,
            Type = 3,
            RelatedId = priceNegotiationId,
            ProductId = productId // Assuming ProductId is not used here
        };

        return await _chatHubService.AddNotificationToUserAsync(notificationViewModel);
    }
}