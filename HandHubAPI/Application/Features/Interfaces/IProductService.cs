using HandHubAPI.Application.DTOs;
using HandHubAPI.Controllers;
using HandHubAPI.Requests;

namespace HandHubAPI.Application.Features.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductRequest request);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductRequest request);
        Task<bool> DeleteProductAsync(int id);
        Task<PaginatedResponse<ProductDto>> GetAllProductsAsync(int pageNumber = 1, int pageSize = 20, int categoryId = 0, int status = 1, string? searchTerm = null);
        Task<PaginatedResponse<CategoryDto>> GetAllCategoriesAsync(int pageNumber = 1, int pageSize = 20);
        Task<PaginatedResponse<SubCategoryDto>> GetAllSubCategoriesAsync(int pageNumber = 1, int pageSize = 20, int categoryId = 0);
        Task<PaginatedResponse<ProductDto>> GetRecentProducts(int pageNumber = 1, int pageSize = 20);
        Task<PaginatedResponse<ProductDto>> GetProductsBySubCategoryAsync(int pageNumber = 1, int pageSize = 20, int subCategoryId = 0, int currentUserId = 0, string? searchTerm = null);
        Task<PaginatedResponse<ProductDto>> GetSuggestedProductsByPurchasedCategoryAsync(int pageNumber = 1, int pageSize = 20, int UserId = 0);
        Task<PaginatedResponse<ProductDto>> GetProductsBySellerWithoutOrderAsync(int pageNumber = 1, int pageSize = 20, int sellerId = 0);
        Task<ProductDto> UpdateProductStatusAsync(int productId, int status);
    }
}