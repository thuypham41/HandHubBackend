using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HandHubAPI.Infrastructure.Repositories;

public class Product_SubcategoryRepository : BaseRepository<Product_SubcategoryEntity>, IProduct_SubcategoryRepository
{
    public Product_SubcategoryRepository(HandHubDbContext context) : base(context)
    {
    }

    public async Task<PaginatedResponse<ProductEntity>> GetProductsBySubCategoryAsync(int PageNumber, int PageSize, int CategoryId, int CurrentUserId, string? SearchTerm)
    {
        var validPageNumber = Math.Max(1, PageNumber);
        var validPageSize = Math.Max(1, Math.Min(100, PageSize));

        var query = _context.Product_Subcategory
            .Where(psc => psc.Product.Status == 1 && !psc.Product.IsDeleted && psc.SubcategoryId == CategoryId)
            .OrderByDescending(psc => psc.CreatedAt)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            query = query.Where(psc => psc.Product.Name.Contains(SearchTerm));
        }

        if (CurrentUserId > 0)
        {
            query = query.Where(psc => psc.Product.SellerId != CurrentUserId);
        }

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((validPageNumber - 1) * validPageSize)
            .Take(validPageSize)
            .Select(psc => psc.Product)
            .ToListAsync();

        return new PaginatedResponse<ProductEntity>
        {
            Items = items,
            PageNumber = validPageNumber,
            PageSize = validPageSize,
            TotalItems = totalItems
        };
    }
    public async Task<Product_SubcategoryEntity?> GetByProductIdAsync(int productId)
    {
        return await _context.Product_Subcategory.FirstOrDefaultAsync(psc => psc.ProductId == productId);
    }
}