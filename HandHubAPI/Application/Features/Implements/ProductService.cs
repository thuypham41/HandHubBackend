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

    public ProductService(
        ILogger<ProductService> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
    {
        try
        {
            var product = new ProductEntity
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CategoryId = request.CategoryId,
                ImageUrl = request.ImageUrl
            };

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.CommitAsync();

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl
            };
        }
        catch (Exception ex)
        {
            _unitOfWork.RollbackTransaction();
            _logger.LogError(ex, "Error occurred while creating product.");
            throw;
        }
    }

    public async Task<PaginatedResponse<ProductDto>> GetRecentProducts(int pageNumber, int pageSize)
    {
        try
        {
            var products = await _unitOfWork.ProductRepository.GetRecentProductsAsync(pageNumber, pageSize);

            var productDtos = products.Items.Select(product => new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl
            }).ToList();

            return new PaginatedResponse<ProductDto>
            {
                Items = productDtos,
                TotalItems = products.TotalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving recent products.");
            throw;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning($"Product with ID {id} not found for deletion.");
                return false;
            }

            await _unitOfWork.ProductRepository.SoftDelete(id);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation($"Product with ID {id} deleted successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollbackTransaction();
            _logger.LogError(ex, $"Error occurred while deleting product with ID {id}.");
            throw;
        }
    }

    public async Task<PaginatedResponse<ProductDto>> GetAllProductsAsync(int pageNumber = 1, int pageSize = 20, int categoryId = 0, string? searchTerm = null)
    {
        try
        {
            var products = await _unitOfWork.ProductRepository.GetPaginatedAsync(pageNumber, pageSize, categoryId, searchTerm);

            var productDtos = products.Items.Select(product => new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl
            }).ToList();

            return new PaginatedResponse<ProductDto>
            {
                Items = productDtos,
                TotalItems = products.TotalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all products.");
            throw;
        }
    }

    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        try
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null)
            {
                return null;
            }
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while retrieving product with ID {id}.");
            throw;
        }
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        try
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning($"Product with ID {id} not found for update.");
                return null;
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.ImageUrl = request.ImageUrl;

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.CommitAsync();

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl
            };
        }
        catch (Exception ex)
        {
            _unitOfWork.RollbackTransaction();
            _logger.LogError(ex, $"Error occurred while updating product with ID {id}.");
            throw;
        }
    }
}
