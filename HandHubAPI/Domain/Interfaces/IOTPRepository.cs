namespace HandHubAPI.Domain.Repositories;

using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;

public interface IOTPRepository : IBaseRepository<OTPEntity>
{
    Task<bool> VerifyOtpAsync(string email, string code);
}