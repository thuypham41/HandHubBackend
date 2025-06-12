using HandHubAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class HandHubDbContext : DbContext
{
    public HandHubDbContext(DbContextOptions<HandHubDbContext> options) : base(options) { }

    public DbSet<RoleEntity> Role { get; set; }
    public DbSet<UserEntity> User { get; set; }
    public DbSet<CategoryEntity> Categorie { get; set; }
    public DbSet<ProductEntity> Product { get; set; }
    public DbSet<PriceNegotiationEntity> PriceNegotiation { get; set; }
    public DbSet<OrderEntity> Order { get; set; }
    public DbSet<OrderDetailEntity> OrderDetail { get; set; }
    public DbSet<NegotiationMessageEntity> NegotiationMessage { get; set; }
    public DbSet<ProductApprovalEntity> ProductApproval { get; set; }
    public DbSet<OTPEntity> OTP { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserEntity>()
            .HasOne<RoleEntity>(m => m.Role)
            .WithMany()
            .HasForeignKey(m => m.RoleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<NegotiationMessageEntity>()
            .HasOne<PriceNegotiationEntity>(m => m.PriceNegotiation)
            .WithMany()
            .HasForeignKey(m => m.PriceNegotiationId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<NegotiationMessageEntity>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<NegotiationMessageEntity>()
            .HasOne<UserEntity>(m => m.Receivier)
            .WithMany()
            .HasForeignKey(m => m.ReceivierId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<OrderDetailEntity>()
            .HasOne(m => m.Product)
            .WithMany()
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<OrderDetailEntity>()
            .HasOne(m => m.Order)
            .WithMany()
            .HasForeignKey(m => m.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<PriceNegotiationEntity>()
            .HasOne(m => m.Product)
            .WithMany()
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<PriceNegotiationEntity>()
            .HasOne(m => m.Buyer)
            .WithMany()
            .HasForeignKey(m => m.BuyerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ProductApprovalEntity>()
            .HasOne(m => m.Product)
            .WithMany()
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ProductApprovalEntity>()
            .HasOne(m => m.Admin)
            .WithMany()
            .HasForeignKey(m => m.AdminId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ProductEntity>()
            .HasOne(m => m.Category)
            .WithMany()
            .HasForeignKey(m => m.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ProductEntity>()
            .HasOne(m => m.Seller)
            .WithMany()
            .HasForeignKey(m => m.SellerId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
