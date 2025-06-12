namespace HandHubAPI.Application.DTOs;

public class OrderDto
{
    public int BuyerId { get; set; }

    public int Price { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; }

    public int Status { get; set; }
}