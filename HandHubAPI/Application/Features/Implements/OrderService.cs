using HandHubAPI.Application.DTOs;
using HandHubAPI.Application.Features.Interfaces;
using HandHubAPI.Domain.Interfaces;

namespace HandHubAPI.Application.Features.Implements;

public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(
        ILogger<OrderService> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    // public Task<OrderDto> CreateOrderAsync(CreateOrderRequest request)
    // {
    //     throw new NotImplementedException();
    // }

    // public Task<bool> DeleteOrderAsync(int id)
    // {
    //     throw new NotImplementedException();
    // }

    public async Task<PaginatedResponse<OrderDto>> GetAllOrdersAsync(int pageNumber, int pageSize, int customerId = 0, string? searchTerm = null)
    {
        try
        {
            var orders = await _unitOfWork.OrderRepository.GetPaginatedAsync(pageNumber, pageSize, customerId, searchTerm);

            // Get all order IDs from the paginated result
            var orderIds = orders.Items.Select(o => o.Id).ToList();

            // Fetch all order details for these orders
            var orderDetails = await _unitOfWork.OrderDetailRepository.GetByOrderIdsAsync(orderIds);

            var orderDtos = new List<OrderDto>();

            foreach (var order in orders.Items)
            {
                var details = orderDetails.Where(d => d.OrderId == order.Id).ToList();
                var productNames = new List<string>();

                foreach (var detail in details)
                {
                    var product = await _unitOfWork.ProductRepository.GetByIdAsync(detail.ProductId);
                    if (product != null)
                    {
                        productNames.Add(product.Name);
                    }
                }

                orderDtos.Add(new OrderDto
                {
                    OrderId = order.Id,
                    BuyerId = order.BuyerId,
                    Price = order.TotalMoney,
                    ProductName = productNames,
                    PaymentMethod = order.PaymentMethod,
                    Address = order.Address,
                    OrderDate = order.OrderDate,
                    Status = order.Status
                });
            }

            return new PaginatedResponse<OrderDto>
            {
                Items = orderDtos,
                TotalItems = orders.TotalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all orders.");
            throw;
        }
    }

    public async Task<PaginatedResponse<OrderDetailDto?>> GetOrderDetailByIdAsync(int orderId)
    {
        try
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return null;
            }

            var orderDetails = await _unitOfWork.OrderDetailRepository.GetByOrderIdAsync(order.Id);
            var products = new List<ProductDto>();
            foreach (var detail in orderDetails)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(detail.ProductId);
                if (product != null)
                {
                    products.Add(new ProductDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        CategoryId = product.CategoryId,
                        ImageUrl = product.ImageUrl
                    });
                }
            }

            return new PaginatedResponse<OrderDetailDto?>
            {
                Items = orderDetails.Select(d => new OrderDetailDto
                {
                    OrderDetailId = d.Id,
                    OrderId = d.OrderId,
                    Product = products.FirstOrDefault(p => p.Id == d.ProductId) ?? new ProductDto(),
                    Quantity = d.Num,
                    Price = d.Price,
                    TotalMoney = d.Num * d.Price
                }).ToList(),
                TotalItems = orderDetails.Count,
                PageNumber = 1,
                PageSize = orderDetails.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while retrieving order details for orderId: {orderId}");
            throw;
        }
    }

    public async Task<PaginatedResponse<OrderSoldDetailDto>> GetAllSoldOrdersByUserIdAsync(int userId, int pageNumber, int pageSize, DateTime? date, int status = 3)
    {
        try
        {
            // var productsSellByUserId = await _unitOfWork.ProductRepository.GetBySellerIdAsync(pageNumber, pageSize, userId);

            // var orderDetails = await _unitOfWork.OrderDetailRepository.GetOrderIdsByProductIdsAsync(productsSellByUserId.Items.Select(p => p.Id).ToList());

            // var orders = await _unitOfWork.OrderRepository.GetByIdsAsync(orderDetails.Select(od => od.OrderId).ToList(), status, date, userId);

            // var orderDtos = new List<OrderSoldDetailDto>();

            // foreach (var order in orders)
            // {
            //     var detail = orderDetails.FirstOrDefault(d => d.OrderId == order.Id);
            //     if (detail != null)
            //     {
            //         var product = productsSellByUserId.Items.FirstOrDefault(p => p.Id == detail.ProductId);

            //         if (product != null)
            //         {
            //             orderDtos.Add(new OrderSoldDetailDto
            //             {
            //                 OrderDetailId = detail.Id,
            //                 OrderId = detail.OrderId,
            //                 Product = new ProductDto
            //                 {
            //                     Id = product.Id,
            //                     Name = product.Name,
            //                     Description = product.Description,
            //                     Price = product.Price,
            //                     CategoryId = product.CategoryId,
            //                     ImageUrl = product.ImageUrl
            //                 },
            //                 Quantity = detail.Num,
            //                 Price = detail.Price,
            //                 TotalMoney = detail.Num * detail.Price,
            //                 Status = order.Status,
            //                 BuyerName = (await _unitOfWork.UserRepository.GetByIdAsync(order.BuyerId))?.FullName ?? "Unknown Buyer"
            //             });
            //         }
            //     }
            // }
            var orderDetails = await _unitOfWork.OrderDetailRepository.GetOrderSoldDetailsBySellerIdAsync(userId, status, date);
            return new PaginatedResponse<OrderSoldDetailDto>
            {
                Items = orderDetails,
                TotalItems = orderDetails.Count,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while retrieving sold orders for userId: {userId}");
            throw;
        }
    }
    // public Task<OrderDto?> GetOrderByIdAsync(int id)
    // {
    //     throw new NotImplementedException();
    // }

    // public Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderRequest request)
    // {
    //     throw new NotImplementedException();
    // }
}