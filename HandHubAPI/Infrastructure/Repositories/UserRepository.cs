using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HandHubAPI.Infrastructure.Repositories;

public class UserRepository : BaseRepository<UserEntity>, IUserRepository
{
    public UserRepository(HandHubDbContext context) : base(context)
    {
    }

    public async Task<UserEntity?> GetByEmailAsync(string Email)
    {
        return await _context.User.FirstOrDefaultAsync(x => x.Email.ToLower() == Email.ToLower());
    }
}