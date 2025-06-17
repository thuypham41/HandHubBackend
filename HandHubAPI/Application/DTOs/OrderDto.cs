namespace HandHubAPI.Application.DTOs;

public class OrderDto
{
    public int OrderId { get; set; }
    public int BuyerId { get; set; }
    public int Price { get; set; }
    public List<string> ProductName { get; set; } = [];
    public string PaymentMethod { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public int Status { get; set; }
}

public class OrderDetailDto
{
    public int OrderDetailId { get; set; }
    public int OrderId { get; set; }
    public ProductDto Product { get; set; } = new();
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalMoney { get; set; }
}

public class OrderSoldDetailDto
{
    public int OrderDetailId { get; set; }
    public int OrderId { get; set; }
    public ProductDto Product { get; set; } = new();
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalMoney { get; set; }
    public int Status { get; set; }
}