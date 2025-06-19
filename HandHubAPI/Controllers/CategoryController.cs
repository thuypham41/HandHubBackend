using System.Net;
using HandHubAPI.Application.DTOs;
using HandHubAPI.Application.Features.Interfaces;
using HandHubAPI.Requests;
using Microsoft.AspNetCore.Mvc;
namespace HandHubAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : BaseController<CategoryController>
{
    private readonly IUserService _userService;
    private readonly IProductService _productService;
    public CategoryController(
        IUserService userService,
        IProductService productService,
        ILogger<CategoryController> logger) : base(logger)
    {
        _userService = userService;
        _productService = productService;
    }

    public class GetAllCategoriesRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetAllSubCategoriesRequest : GetAllCategoriesRequest
    {
        public int CategoryId { get; set; }
    }

    [HttpGet("get-all-categories")]
    public async Task<IActionResult> GetAllCategoriesAsync([FromQuery] GetAllCategoriesRequest request)
    {
        try
        {
            var products = await _productService.GetAllCategoriesAsync(request.PageNumber, request.PageSize);
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
            return ErrorResponse("Failed to retrieve categories", HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("get-all-sub-categories")]
    public async Task<IActionResult> GetAllSubCategoriesAsync([FromQuery] GetAllSubCategoriesRequest request)
    {
        try
        {
            var products = await _productService.GetAllSubCategoriesAsync(request.PageNumber, request.PageSize, request.CategoryId);
            return PaginatedResponse(
                products.Items,
                products.PageNumber,
                products.PageSize,
                products.TotalItems,
                "Sub-categories retrieved successfully"
            );
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to retrieve categories", HttpStatusCode.InternalServerError, ex);
        }
    }
}