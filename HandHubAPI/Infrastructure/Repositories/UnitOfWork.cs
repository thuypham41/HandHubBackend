using HandHubAPI.Domain.Interfaces;
using HandHubAPI.Domain.Repositories;

namespace HandHubAPI.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly HandHubDbContext _context;

    public UnitOfWork(HandHubDbContext context)
    {
        _context = context;
    }

    private IUserRepository? _users;
    private IProductRepository? _products;
    private IOrderRepository? _orders;
    private ICategoryRepository? _categories;
    private IOrderDetailRepository? _orderDetails;
    private IPriceNegotiationRepository? _priceNegotiations;
    private IProductApprovalRepository? _productApprovals;
    private IRoleRepository? _roles;
    private INegotiationMessageRepository? _negotiationMessages;
    private IOTPRepository? _otpRepository;

    public IUserRepository UserRepository => _users ??= new UserRepository(_context);
    public IProductRepository ProductRepository => _products ??= new ProductRepository(_context);
    public IOrderRepository OrderRepository => _orders ??= new OrderRepository(_context);
    public ICategoryRepository CategoryRepository => _categories ??= new CategoryRepository(_context);
    public IOrderDetailRepository OrderDetailRepository => _orderDetails ??= new OrderDetailRepository(_context);
    public IPriceNegotiationRepository PriceNegotiationRepository => _priceNegotiations ??= new PriceNegotiationRepository(_context);
    public IProductApprovalRepository ProductApprovalRepository => _productApprovals ??= new ProductApprovalRepository(_context);
    public IRoleRepository RoleRepository => _roles ??= new RoleRepository(_context);
    public INegotiationMessageRepository NegotiationMessageRepository => _negotiationMessages ??= new NegotiationMessageRepository(_context);
    public IOTPRepository OTPRepository => _otpRepository ??= new OTPRepository(_context);

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public void BeginTransaction()
    {
        // Implement transaction logic here
    }

    public void RollbackTransaction()
    {
        // Implement rollback logic here
    }
}