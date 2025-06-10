using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;

namespace HandHubAPI.Infrastructure.Repositories;

public class ProductRepository : BaseRepository<ProductEntity>, IProductRepository
{
    public ProductRepository(HandHubDbContext context) : base(context)
    {
    }
}