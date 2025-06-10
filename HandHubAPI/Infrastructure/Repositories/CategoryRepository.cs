using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;

namespace HandHubAPI.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository<CategoryEntity>, ICategoryRepository
{
    public CategoryRepository(HandHubDbContext context) : base(context)
    {
    }
}