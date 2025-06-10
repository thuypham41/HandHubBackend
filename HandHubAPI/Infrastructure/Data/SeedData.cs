using Microsoft.EntityFrameworkCore;
using HandHubAPI.Domain.Entities;
namespace HandHubAPI.Infrastructure.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new HandHubDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<HandHubDbContext>>()))
        {
            // Kiểm tra nếu đã có người dùng thì không seed lại
            if (context.User.Any())
            {
                return;
            }

            var random = new Random();

            // Seed Roles
            var roles = new List<RoleEntity>
            {
                new RoleEntity { Name = "Admin" },
                new RoleEntity { Name = "User" }
            };
            context.Role.AddRange(roles);
            context.SaveChanges();

            // Seed Users
            var users = new List<UserEntity>();
            for (int i = 1; i <= 20; i++)
            {
                users.Add(new UserEntity
                {
                    FullName = $"User {i}",
                    Email = $"user{i}@example.com",
                    DateOfBirth = DateTime.Today.AddYears(-20).AddDays(i),
                    Address = $"123 Street {i}",
                    PhoneNumber = $"09000000{i:D2}",
                    RoleId = roles[1].Id
                });
            }
            context.User.AddRange(users);
            context.SaveChanges();

            // Seed Categories
            var categories = new List<CategoryEntity>();
            for (int i = 1; i <= 10; i++)
            {
                categories.Add(new CategoryEntity
                {
                    Name = $"Category {i}"
                });
            }
            context.Categorie.AddRange(categories);
            context.SaveChanges();

            // Seed Products
            var products = new List<ProductEntity>();
            foreach (var category in categories)
            {
                for (int i = 1; i <= 20; i++)
                {
                    var seller = users[random.Next(users.Count)];
                    products.Add(new ProductEntity
                    {
                        CategoryId = category.Id,
                        SellerId = seller.Id,
                        Name = $"Product {category.Name} - {i}",
                        Condition = "New",
                        Price = random.Next(100, 1000) * 1000,
                        Description = $"This is a sample description for product {i} in {category.Name}",
                        ImageUrl = "https://via.placeholder.com/150",
                        CreatedAt = DateTime.Now.AddDays(-random.Next(10)),
                        UpdatedAt = DateTime.Now,
                        Status = 1
                    });
                }
            }

            context.Product.AddRange(products);
            context.SaveChanges();
            // Seed Orders
            var orders = new List<OrderEntity>();
            for (int i = 1; i <= 10; i++)
            {
                var buyer = users[random.Next(users.Count)];
                var order = new OrderEntity
                {
                    BuyerId = buyer.Id,
                    Price = 0,
                    PaymentMethod = "COD",
                    Address = buyer.Address,
                    OrderDate = DateTime.Now.AddDays(-i),
                    Status = 0, // pending
                    TotalMoney = 0
                };

                // Mỗi order có 1-3 sản phẩm
                var orderDetails = new List<OrderDetailEntity>();
                var orderProducts = products.OrderBy(p => Guid.NewGuid()).Take(random.Next(1, 4)).ToList();

                foreach (var prod in orderProducts)
                {
                    var quantity = random.Next(1, 5);
                    var detail = new OrderDetailEntity
                    {
                        ProductId = prod.Id,
                        Price = prod.Price,
                        Num = quantity,
                        TotalMoney = prod.Price * quantity
                    };
                    orderDetails.Add(detail);
                    order.TotalMoney += detail.TotalMoney;
                }

                orders.Add(order);
            }
            context.Order.AddRange(orders);
            context.SaveChanges();

            // Seed Price Negotiations
            var negotiations = new List<PriceNegotiationEntity>();
            for (int i = 1; i <= 10; i++)
            {
                var product = products[random.Next(products.Count)];
                var buyer = users.Where(u => u.Id != product.SellerId).OrderBy(u => Guid.NewGuid()).First();

                negotiations.Add(new PriceNegotiationEntity
                {
                    ProductId = product.Id,
                    BuyerId = buyer.Id,
                    OfferPrice = product.Price - random.Next(10, 100) * 1000,
                    Status = 0,
                    CreatedAt = DateTime.Now.AddDays(-i),
                    SellerResponse = "Waiting",
                    FinalPrice = 0
                });
            }
            context.PriceNegotiation.AddRange(negotiations);
            context.SaveChanges();

            // Seed Negotiation Messages
            var messages = new List<NegotiationMessageEntity>();
            foreach (var nego in negotiations)
            {
                var buyer = users.FirstOrDefault(u => u.Id == nego.BuyerId);
                var seller = users.FirstOrDefault(u => u.Id == products.First(p => p.Id == nego.ProductId).SellerId);

                messages.Add(new NegotiationMessageEntity
                {
                    PriceNegotiationId = nego.Id,
                    SenderId = buyer.Id,
                    ReceivierId = seller.Id,
                    MessageContent = "Hi, can you lower the price?"
                });

                messages.Add(new NegotiationMessageEntity
                {
                    PriceNegotiationId = nego.Id,
                    SenderId = seller.Id,
                    ReceivierId = buyer.Id,
                    MessageContent = "How about a small discount?"
                });
            }
            context.NegotiationMessage.AddRange(messages);
            context.SaveChanges();
        }
    }
}