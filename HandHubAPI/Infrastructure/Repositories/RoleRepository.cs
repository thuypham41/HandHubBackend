using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;

namespace HandHubAPI.Infrastructure.Repositories;

public class RoleRepository : BaseRepository<RoleEntity>, IRoleRepository
{
    public RoleRepository(HandHubDbContext context) : base(context)
    {
    }
}