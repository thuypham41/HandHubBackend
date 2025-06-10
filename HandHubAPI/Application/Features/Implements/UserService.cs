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
}