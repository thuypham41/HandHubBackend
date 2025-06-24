using HandHubAPI.Application.Features.Implements;
using HandHubAPI.Application.Features.Interfaces;
using HandHubAPI.Domain.Interfaces;
using HandHubAPI.Domain.Repositories;
using HandHubAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HandHubAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        //Regis DbContext
        services.AddDbContext<HandHubDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        //register repository
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
        services.AddScoped<IPriceNegotiationRepository, PriceNegotiationRepository>();
        services.AddScoped<IProductApprovalRepository, ProductApprovalRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartItemRepository, CartItemRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        //register services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IChatHubService, ChatHubService>();
        services.AddScoped<IPriceNegotiationService, PriceNegotiationService>();
        return services;
    }
}