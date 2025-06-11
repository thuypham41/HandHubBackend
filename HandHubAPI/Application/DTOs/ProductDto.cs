namespace HandHubAPI.Application.DTOs;

public class ProductDto
{
    public int CategoryId { get; set; }
    public int SellerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public int Price { get; set; }
    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;
    public int Status { get; set; }

}