using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;

namespace HandHubAPI.Infrastructure.Repositories;

public class UserRepository : BaseRepository<UserEntity>, IUserRepository
{
    public UserRepository(HandHubDbContext context) : base(context)
    {
    }
}