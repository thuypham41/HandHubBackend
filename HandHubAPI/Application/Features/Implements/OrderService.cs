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

    // public Task<OrderDto?> GetOrderByIdAsync(int id)
    // {
    //     throw new NotImplementedException();
    // }

    // public Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderRequest request)
    // {
    //     throw new NotImplementedException();
    // }
}