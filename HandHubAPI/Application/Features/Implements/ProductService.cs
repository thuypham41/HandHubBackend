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

    public async Task<PaginatedResponse<ProductDto>> GetProductsBySellerWithoutOrderAsync(int pageNumber, int pageSize, int sellerId)
    {
        try
        {
            var products = await _unitOfWork.ProductRepository.GetProductsBySellerWithoutOrderAsync(pageNumber, pageSize, sellerId);

            var productDtos = products.Items.Select(product => new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl,
                CreatedAt = product.CreatedAt,
                IsDeleted = product.IsDeleted
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
            _logger.LogError(ex, $"Error occurred while retrieving products by seller without order for seller ID {sellerId}.");
            throw;
        }
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

    public async Task<List<string>> GetAllCloudinaryImagesAsync(string cloudName, string apiKey, string apiSecret)
    {
        var imageUrls = new List<string>();
        try
        {
            var account = new CloudinaryDotNet.Account(cloudName, apiKey, apiSecret);
            var cloudinary = new CloudinaryDotNet.Cloudinary(account);

            var resources = await cloudinary.ListResourcesAsync(new CloudinaryDotNet.Actions.ListResourcesParams
            {
                ResourceType = CloudinaryDotNet.Actions.ResourceType.Image,
                MaxResults = 500
            });

            if (resources.Resources != null)
            {
                imageUrls.AddRange(resources.Resources.Select(r => r.SecureUrl.ToString()));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving images from Cloudinary.");
            throw;
        }
        return imageUrls;
    }

    public async Task<PaginatedResponse<ProductDto>> GetProductsBySubCategoryAsync(int pageNumber = 1, int pageSize = 20, int subCategoryId = 0, string? searchTerm = null)
    {
        try
        {
            var products = await _unitOfWork.Product_SubcategoryRepository.GetProductsBySubCategoryAsync(pageNumber, pageSize, subCategoryId, searchTerm);

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
            _logger.LogError(ex, "Error occurred while retrieving products by subcategory.");
            throw;
        }
    }

    public async Task<PaginatedResponse<ProductDto>> GetSuggestedProductsByPurchasedCategoryAsync(int pageNumber = 1, int pageSize = 20, int userId = 0)
    {
        try
        {
            // Get the list of category IDs the user has purchased from
            var purchasedCategoryIds = await _unitOfWork.OrderRepository.GetPurchasedCategoryIdsByUserAsync(userId);

            if (purchasedCategoryIds == null || !purchasedCategoryIds.Any())
            {
                return new PaginatedResponse<ProductDto>
                {
                    Items = new List<ProductDto>(),
                    TotalItems = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            // Get products from those categories, excluding user's own purchases if needed
            var products = await _unitOfWork.ProductRepository.GetProductsByCategoryIdsAsync(
                 pageNumber, pageSize, purchasedCategoryIds.ToList());

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
            _logger.LogError(ex, "Error occurred while retrieving suggested products by purchased category.");
            throw;
        }
    }

    public async Task<PaginatedResponse<CategoryDto>> GetAllCategoriesAsync(int pageNumber, int pageSize)
    {
        try
        {
            var categories = await _unitOfWork.CategoryRepository.GetPaginatedAsync(pageNumber, pageSize);

            var categoryDtos = categories.Items.Select(category => new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            }).ToList();

            return new PaginatedResponse<CategoryDto>
            {
                Items = categoryDtos,
                TotalItems = categories.TotalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all categories.");
            throw;
        }
    }

    public async Task<PaginatedResponse<SubCategoryDto>> GetAllSubCategoriesAsync(int pageNumber, int pageSize, int categoryId = 0)
    {
        try
        {
            var subCategories = await _unitOfWork.SubCategoryRepository.GetPaginatedAsync(pageNumber, pageSize);

            var subCategoryDtos = subCategories.Items
                .Where(x => x.CategoryId == categoryId)
                .Select(subCategory => new SubCategoryDto
                {
                    Id = subCategory.Id,
                    Name = subCategory.Name,
                    CategoryId = subCategory.CategoryId
                }).ToList();

            return new PaginatedResponse<SubCategoryDto>
            {
                Items = subCategoryDtos,
                TotalItems = subCategories.TotalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all subcategories.");
            throw;
        }
    }
}