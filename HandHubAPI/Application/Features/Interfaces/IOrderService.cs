using HandHubAPI.Application.DTOs;

namespace HandHubAPI.Application.Features.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<OrderDto> CreateOrderAsync(CreateOrderRequest request);
        Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderRequest request);
        Task<bool> DeleteOrderAsync(int id);
        Task<PaginatedResult<OrderDto>> GetAllOrdersAsync(int pageNumber, int pageSize, int customerId = 0, string? searchTerm = null);
    }
}