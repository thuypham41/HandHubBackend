using HandHubAPI.Application.DTOs;

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
    }
}