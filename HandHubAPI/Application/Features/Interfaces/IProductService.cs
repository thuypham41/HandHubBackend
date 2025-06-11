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
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    }
}