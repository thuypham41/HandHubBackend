using HandHubAPI.Application.DTOs;
using HandHubAPI.Application.Features.Interfaces;
using HandHubAPI.Domain.Interfaces;
using HandHubAPI.Controllers;
using HandHubAPI.Domain.Entities;
using static HandHubAPI.Controllers.OrderController;

namespace HandHubAPI.Application.Features.Implements;

public class OrderService : IOrderService
{
    public ICartService CartService { get; set; } = null!;
    private readonly ILogger<OrderService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(
        ILogger<OrderService> logger,
        ICartService cartService,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        CartService = cartService;
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderRequest request)
    {
        try
        {
            // Implementation for creating an order
            // This is a basic implementation - adjust according to your domain model
            var order = new OrderEntity
            {
                BuyerId = request.CustomerId,
                TotalMoney = request.TotalMoney,
                PaymentMethod = request.PaymentMethod,
                Address = request.ShippingAddress,
                OrderDate = DateTime.UtcNow,
                Status = 0, // 0: Đang xử lý, 1: Đang giao hàng, 2: Đã giao hàng, 3: Tất cả, -1: Đã hủy
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add order first to get the order ID
            var orderAdded = await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.CommitAsync();

            // Add order items
            foreach (var item in request.Items)
            {
                var orderDetail = new OrderDetailEntity
                {
                    OrderId = orderAdded.Id,
                    ProductId = item.ProductId,
                    Num = item.Quantity,
                    Price = item.Price
                };

                await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
            }

            await CartService.ClearAllCartbyUserIdAsync(request.CustomerId);
            foreach (var item in request.Items)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.Status = 2;
                    _unitOfWork.ProductRepository.Update(product);
                }
            }

            await _unitOfWork.CommitAsync();

            _logger.LogInformation($"Order created successfully with ID: {orderAdded.Id}");

            return new OrderDto
            {
                OrderId = orderAdded.Id,
                BuyerId = orderAdded.BuyerId,
                Price = orderAdded.TotalMoney,
                PaymentMethod = orderAdded.PaymentMethod,
                Address = orderAdded.Address,
                OrderDate = orderAdded.OrderDate,
                Status = orderAdded.Status
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating order");
            throw;
        }
    }
    public async Task<List<GetSoldOrdersInDateRangeResponse>> GetSoldOrdersInDateRange(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var users = await _unitOfWork.OrderRepository.GetUsersWithSoldOrdersInDateRange(startDate, endDate);
            List<GetSoldOrdersInDateRangeResponse> result = [];
            foreach (var user in users)
            {
                result.Add(new GetSoldOrdersInDateRangeResponse
                {
                    UserName = user.FullName,
                    userId = user.Id,
                    TotalOrders = await _unitOfWork.OrderRepository.GetTotalOrdersByUser(user.Id, startDate, endDate),
                    TotalRevenue = await _unitOfWork.OrderRepository.GetTotalRevenueByUser(user.Id, startDate, endDate),
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving sold orders in date range.");
            throw;
        }
    }

    public async Task<decimal> GetAllRevenueAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            return await _unitOfWork.OrderRepository.GetTotalRevenueAsync(startDate, endDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while calculating revenue.");
            throw;
        }
    }

    public async Task<OrderDto> CancelOrderAsync(int orderId, string reason)
    {
        try
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning($"Order with ID {orderId} not found for cancellation.");
                return null;
            }

            // Assuming there is a Status property and a CancelReason property
            order.Status = -1; // -1 indicates cancelled
            order.CancelReason = reason;
            order.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation($"Order with ID {orderId} cancelled successfully.");
            return new OrderDto
            {
                OrderId = order.Id,
                Status = order.Status,
                BuyerId = order.BuyerId,
                OrderDate = order.OrderDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while cancelling order with ID: {orderId}");
            throw;
        }
    }

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

    public async Task<List<TotalRevenueByWeekInMonthResponse>> GetTotalRevenueByWeekInMonth(int month, int year)
    {
        try
        {
            var weeklyRevenues = new List<TotalRevenueByWeekInMonthResponse>();

            // Get revenue for each week (1-4) in the specified month
            for (int week = 1; week <= 4; week++)
            {
                var weekRevenue = await _unitOfWork.OrderRepository.GetTotalRevenueByWeekInMonth(month, year, week);
                weeklyRevenues.Add(new TotalRevenueByWeekInMonthResponse
                {
                    Week = week,
                    TotalRevenue = weekRevenue
                });
            }

            return weeklyRevenues;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting total revenue by week in month {month}/{year}");
            throw;
        }
    }
}