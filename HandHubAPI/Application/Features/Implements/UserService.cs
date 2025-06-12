using HandHubAPI.Application.DTOs;
using HandHubAPI.Application.Features.Interfaces;
using HandHubAPI.Controllers;
using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;
using HandHubAPI.Requests;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace HandHubAPI.Application.Features.Implements;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public UserService(
        ILogger<UserService> logger,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
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

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email))
            return false;

        var email = _unitOfWork.UserRepository.GetByEmailAsync(request.Email);
        if (email == null)
            return false;
        return await SendOTPAsync(request.Email, GenerateOtp());
    }

    public async Task<bool> SendOTPAsync(string toEmail, string otp)
    {
        string subject = "Mã xác thực OTP - HandHub";
        string htmlBody = GenerateOtpEmailMessage(toEmail, otp);

        try
        {
            var fromAddress = new MailAddress(_configuration["SendMail:FromAddress"] ?? "", "HandHub");
            var toAddress = new MailAddress(toEmail);
            string fromPassword = _configuration["SendMail:FromAddressPassword"] ?? "";

            var smtp = new SmtpClient
            {
                Host = _configuration["SendMail:Host"] ?? "",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(message);
            var otpCode = new OTPEntity
            {
                Email = toEmail,
                Code = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };
            await _unitOfWork.OTPRepository.AddAsync(otpCode);
            await _unitOfWork.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
    private string GenerateOtpEmailMessage(string userName, string otpCode)
    {
        return $@"
          <html>
              <head>
                  <style>
                      .otp-container {{
                          font-family: Arial, sans-serif;
                          max-width: 600px;
                          margin: auto;
                          border: 1px solid #eee;
                          padding: 20px;
                          border-radius: 10px;
                          background-color: #f9f9f9;
                      }}
                      .otp-code {{
                          font-size: 24px;
                          font-weight: bold;
                          color: #2c3e50;
                      }}
                      .footer {{
                          margin-top: 20px;
                          font-size: 12px;
                          color: #888;
                      }}
                  </style>
              </head>
              <body>
                  <div class='otp-container'>
                      <h2>Xác minh tài khoản HandHub</h2>
                      <p>Xin chào <strong>{userName}</strong>,</p>
                      <p>Cảm ơn bạn đã sử dụng HandHub. Mã xác thực OTP của bạn là:</p>
                      <p class='otp-code'>{otpCode}</p>
                      <p>Vui lòng nhập mã này vào ứng dụng để hoàn tất quá trình xác minh.</p>
                      <p class='footer'>Mã OTP sẽ hết hạn sau 5 phút. Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email.</p>
                  </div>
              </body>
          </html>";
    }

    private string GenerateOtp()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    public Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SignUpAsync(SignupRequest request)
    {
        if (string.IsNullOrEmpty(request.Email))
            return false;

        var existingUser = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            _logger.LogWarning($"Email {request.Email} is already registered.");
            return false;
        }

        var newUser = new UserEntity
        {
            Email = request.Email,
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            Address = request.Address,
            PhoneNumber = request.PhoneNumber,
            RoleId = 2
        };

        await _unitOfWork.UserRepository.AddAsync(newUser);
        await _unitOfWork.CommitAsync();

        return await SendOTPAsync(request.Email, GenerateOtp());
    }

    public async Task<bool> LogoutAsync()
    {
        // Assuming logout involves invalidating a session or token
        try
        {
            // Perform any necessary cleanup or token invalidation logic here
            _logger.LogInformation("User logged out successfully.");
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred during logout: {ex.Message}");
            return await Task.FromResult(false);
        }
    }

    public async Task<UserEntity> VerifyOtpAsync(VerifyOtpRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Otp))
            return null;

        var user = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email);
        if (user == null)
            return null;

        var isOtpValid = await _unitOfWork.OTPRepository.VerifyOtpAsync(request.Email, request.Otp);
        if (!isOtpValid)
        {
            _logger.LogWarning($"Invalid OTP for email {request.Email}.");
            return null;
        }

        _logger.LogInformation($"OTP verified successfully for email {request.Email}.");
        return user;
    }
}