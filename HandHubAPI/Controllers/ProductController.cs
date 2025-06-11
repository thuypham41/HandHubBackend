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

    [HttpGet("get-product-by-id")]
    public async Task<IActionResult> GetProductById(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
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

    [HttpPut("update-product/{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            var product = await _productService.UpdateProductAsync(id, request);
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
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var result = await _productService.DeleteProductAsync(id);
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
    public async Task<IActionResult> GetAllProducts()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            return CommonResponse(products, "Products retrieved successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to retrieve products", HttpStatusCode.InternalServerError, ex);
        }
    }
}