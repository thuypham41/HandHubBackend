namespace HandHubAPI.Domain.Repositories;

using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;

public interface IProduct_SubcategoryRepository : IBaseRepository<Product_SubcategoryEntity>
{
    Task<PaginatedResponse<ProductEntity>> GetProductsBySubCategoryAsync(int PageNumber, int PageSize, int CategoryId, string? SearchTerm);
}