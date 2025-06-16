using System.Net;
using HandHubAPI.Application.DTOs;
using HandHubAPI.Application.Features.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace HandHubAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : BaseController<ProductController>
{
    private readonly IProductService _productService;
    public ProductController(IProductService productService, ILogger<ProductController> logger) : base(logger)
    {
        _productService = productService;
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

    [HttpDelete("delete-product/{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] DeleteProductRequest request)
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
    }

    [HttpGet("get-subcategory-products")]
    public async Task<IActionResult> GetSubCategoryProducts([FromQuery] GetSubCategoryProductsRequest request)
    {
        try
        {
            var products = await _productService.GetProductsBySubCategoryAsync(
                request.PageNumber,
                request.PageSize,
                request.SubCategoryId
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

}