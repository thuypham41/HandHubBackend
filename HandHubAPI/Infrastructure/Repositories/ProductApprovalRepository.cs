using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;

namespace HandHubAPI.Infrastructure.Repositories;

public class ProductApprovalRepository : BaseRepository<ProductApprovalEntity>, IProductApprovalRepository
{
    public ProductApprovalRepository(HandHubDbContext context) : base(context)
    {
    }
}