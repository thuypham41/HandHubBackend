using System.Net;
using HandHubAPI.Application.DTOs;
using HandHubAPI.Application.Features.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace HandHubAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : BaseController<UserController>
{
    private readonly IUserService _userService;
    public UserController(
        IUserService userService,
        ILogger<UserController> logger) : base(logger)
    {
        _userService = userService;
    }

    [HttpGet("get-user-by-id")]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return ErrorResponse("User not found", HttpStatusCode.NotFound);
            }
            return CommonResponse(user, "User retrieved successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to retrieve user", HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpDelete("delete-user/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return ErrorResponse("User not found", HttpStatusCode.NotFound);
            }
            return CommonResponse(result, "User deleted successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to delete user", HttpStatusCode.InternalServerError, ex);
        }
    }

    // [HttpGet("get-paginated-users")]
    // public async Task<IActionResult> GetPaginatedUsers([FromQuery] PaginationRequest paginationRequest)
    // {
    //     try
    //     {
    //         var users = await _userService.GetPaginatedUsersAsync(paginationRequest);
    //         return CommonResponse(users, "Users retrieved successfully");
    //     }
    //     catch (Exception ex)
    //     {
    //         return ErrorResponse("Failed to retrieve users", HttpStatusCode.InternalServerError, ex);
    //     }
    // }

    [HttpPut("update-user/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto request)
    {
        try
        {
            var user = await _userService.UpdateUserAsync(id, request);
            if (user == null)
            {
                return ErrorResponse("User not found", HttpStatusCode.NotFound);
            }
            return CommonResponse(user, "User updated successfully");
        }
        catch (Exception ex)
        {
            return ErrorResponse("Failed to update user", HttpStatusCode.InternalServerError, ex);
        }
    }

    // [HttpPost("login")]
    // public async Task<IActionResult> Login([FromBody] LoginRequest request)
    // {
    //     try
    //     {
    //         var token = await _userService.LoginAsync(request);
    //         if (token == null)
    //         {
    //             return ErrorResponse("Invalid credentials", HttpStatusCode.Unauthorized);
    //         }
    //         return CommonResponse(token, "Login successful");
    //     }
    //     catch (Exception ex)
    //     {
    //         return ErrorResponse("Failed to login", HttpStatusCode.InternalServerError, ex);
    //     }
    // }

    // [HttpPost("signup")]
    // public async Task<IActionResult> Signup([FromBody] SignupRequest request)
    // {
    //     try
    //     {
    //         var user = await _userService.SignupAsync(request);
    //         return CommonResponse(user, "Signup successful");
    //     }
    //     catch (Exception ex)
    //     {
    //         return ErrorResponse("Failed to signup", HttpStatusCode.InternalServerError, ex);
    //     }
    // }

    // [HttpPost("logout")]
    // public async Task<IActionResult> Logout()
    // {
    //     try
    //     {
    //         await _userService.LogoutAsync();
    //         return CommonResponse(null, "Logout successful");
    //     }
    //     catch (Exception ex)
    //     {
    //         return ErrorResponse("Failed to logout", HttpStatusCode.InternalServerError, ex);
    //     }
    // }
}