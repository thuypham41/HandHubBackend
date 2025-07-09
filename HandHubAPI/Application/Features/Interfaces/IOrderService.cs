using HandHubAPI.Application.DTOs;
using static HandHubAPI.Controllers.OrderController;

namespace HandHubAPI.Application.Features.Interfaces
{
    public interface IOrderService
    {
        // Task<OrderDto?> GetOrderByIdAsync(int id);
        // Task<OrderDto> CreateOrderAsync(CreateOrderRequest request);
        // Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderRequest request);
        // Task<bool> DeleteOrderAsync(int id);
        Task<PaginatedResponse<OrderDto>> GetAllOrdersAsync(int pageNumber, int pageSize, int customerId = 0, string? searchTerm = null);
        Task<PaginatedResponse<OrderSoldDetailDto>> GetAllSoldOrdersByUserIdAsync(int userId, int pageNumber, int pageSize, DateTime? date, int status = 3); // 3 means all status
        Task<PaginatedResponse<OrderDetailDto?>> GetOrderDetailByIdAsync(int orderId);
        Task<OrderDto?> CancelOrderAsync(int orderId, string message);
        Task<OrderDto?> CreateOrderAsync(CreateOrderRequest orderDto);

        Task<decimal> GetAllRevenueAsync(DateTime? startDate, DateTime? endDate);
        Task<List<GetSoldOrdersInDateRangeResponse>> GetSoldOrdersInDateRange(DateTime? startDate, DateTime? endDate);
        Task<List<TotalRevenueByWeekInMonthResponse>> GetTotalRevenueByWeekInMonth(int month, int year);
    }
}