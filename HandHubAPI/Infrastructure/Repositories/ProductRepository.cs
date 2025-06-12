using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HandHubAPI.Infrastructure.Repositories;

public class ProductRepository : BaseRepository<ProductEntity>, IProductRepository
{
    public ProductRepository(HandHubDbContext context) : base(context)
    {
    }

    public async Task<PaginatedResponse<ProductEntity>> GetPaginatedAsync(int PageNumber, int PageSize, int CategoryId, string? SearchTerm)
    {
        var validPageNumber = Math.Max(1, PageNumber);
        var validPageSize = Math.Max(1, Math.Min(100, PageSize));

        var query = _context.Product.AsQueryable();

        if (CategoryId > 0)
        {
            query = query.Where(p => p.CategoryId == CategoryId);
        }

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            query = query.Where(p => p.Name.ToLower().Contains(SearchTerm.ToLower()));
        }

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((validPageNumber - 1) * validPageSize)
            .Take(validPageSize)
            .ToListAsync();

        return new PaginatedResponse<ProductEntity>
        {
            Items = items,
            PageNumber = validPageNumber,
            PageSize = validPageSize,
            TotalItems = totalItems
        };
    }
}