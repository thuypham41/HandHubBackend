using HandHubAPI.Application.DTOs;
using HandHubAPI.Application.Features.Interfaces;
using HandHubAPI.Controllers;
using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;

namespace HandHubAPI.Application.Features.Implements;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        ILogger<UserService> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        var user = new UserEntity
        {
            FullName = createUserDto.FullName,
            Email = createUserDto.Email,
            DateOfBirth = createUserDto.DateOfBirth,
            Address = createUserDto.Address,
            PhoneNumber = createUserDto.PhoneNumber,
            RoleId = createUserDto.RoleId
        };

        await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.CommitAsync();

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            Address = user.Address,
            PhoneNumber = user.PhoneNumber,
            RoleId = user.RoleId
        };
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning($"User with ID {userId} not found for deletion.");
            return false;
        }

        await _unitOfWork.UserRepository.SoftDelete(userId);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation($"User with ID {userId} deleted successfully.");
        return true;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 20)
    {
        var users = await _unitOfWork.UserRepository.GetPaginatedAsync(page, pageSize);
        return users.Items.Select(user => new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            Address = user.Address,
            PhoneNumber = user.PhoneNumber,
            RoleId = user.RoleId
        });
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        return user != null ? new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            Address = user.Address,
            PhoneNumber = user.PhoneNumber,
            RoleId = user.RoleId
        } : null;
    }

    public Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
    {
        throw new NotImplementedException();
    }
}