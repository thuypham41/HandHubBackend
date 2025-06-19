using HandHubAPI.Domain.Repositories;

namespace HandHubAPI.Domain.Interfaces;
public interface IUnitOfWork : IDisposable
{
    IUserRepository UserRepository { get; }
    IProductRepository ProductRepository { get; }
    IOrderRepository OrderRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IOrderDetailRepository OrderDetailRepository { get; }
    IPriceNegotiationRepository PriceNegotiationRepository { get; }
    IProductApprovalRepository ProductApprovalRepository { get; }
    IRoleRepository RoleRepository { get; }
    INegotiationMessageRepository NegotiationMessageRepository { get; }
    IProduct_SubcategoryRepository Product_SubcategoryRepository { get; }
    IOTPRepository OTPRepository { get; }
    ISubCategoryRepository SubCategoryRepository { get; }
    void BeginTransaction();
    void RollbackTransaction();
    Task<int> CommitAsync();
}