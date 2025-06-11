using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HandHubAPI.Infrastructure.Repositories;

public class OTPRepository : BaseRepository<OTPEntity>, IOTPRepository
{
    public OTPRepository(HandHubDbContext context) : base(context)
    {
    }

    public async Task<bool> VerifyOtpAsync(string email, string code)
    {
        var otpEntity = await _context.Set<OTPEntity>().FirstOrDefaultAsync(e => e.Email == email && e.Code == code);

        if (otpEntity == null || otpEntity.ExpiresAt < DateTime.UtcNow)
            return false;

        return true;
    }
}