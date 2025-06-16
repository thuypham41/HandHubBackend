namespace HandHubAPI.Domain.Repositories;

using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;

public interface IProductRepository : IBaseRepository<ProductEntity>
{
     Task<PaginatedResponse<ProductEntity>> GetPaginatedAsync(int PageNumber, int PageSize, int CategoryId, string? SearchTerm);
     Task<PaginatedResponse<ProductEntity>> GetRecentProductsAsync(int PageNumber, int PageSize);
     Task<PaginatedResponse<ProductEntity>> GetProductsByCategoryIdsAsync(int PageNumber, int PageSize, IEnumerable<int> categoryIds);
}