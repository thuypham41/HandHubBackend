namespace HandHubAPI.Application.DTOs;

public class UpdateProductRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public int Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int Status { get; set; }
    public bool IsDeleted { get; set; } = false;
    public int CategoryId { get; set; }
    public int SubCategoryId { get; set; }
}