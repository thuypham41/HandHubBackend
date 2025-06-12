using HandHubAPI.Application.DTOs;
using HandHubAPI.Controllers;
using HandHubAPI.Domain.Entities;
using HandHubAPI.Requests;

namespace HandHubAPI.Application.Features.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<IEnumerable<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 20);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> LoginAsync(LoginRequest request);
        Task<bool> SignUpAsync(SignupRequest request);
        Task<bool> LogoutAsync();
        Task<UserEntity> VerifyOtpAsync(VerifyOtpRequest request);
    }
}