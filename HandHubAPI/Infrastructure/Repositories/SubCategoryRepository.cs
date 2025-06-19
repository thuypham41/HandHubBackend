using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;

namespace HandHubAPI.Infrastructure.Repositories;

public class SubCategoryRepository : BaseRepository<SubCategoryEntity>, ISubCategoryRepository
{
    public SubCategoryRepository(HandHubDbContext context) : base(context)
    {
    }
}