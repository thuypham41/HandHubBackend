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

public class ProductService : IProductService
{
    private readonly ILogger<ProductService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public ProductService(
        ILogger<ProductService> logger,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public Task<ProductDto> CreateProductAsync(CreateProductRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteProductAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ProductDto> GetProductByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ProductDto?> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        throw new NotImplementedException();
    }
}
