using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandHubAPI.Domain.Entities;

public class ProductEntity : BaseEntity
{
    public int CategoryId { get; set; }

    public int SellerId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Condition { get; set; } = string.Empty;

    public int Price { get; set; }

    public string Description { get; set; } = string.Empty;

    [MaxLength(255)]
    public string ImageUrl { get; set; } = string.Empty;
    public int Status { get; set; } // 0: chờ duyệt, 1: đã duyệt, 2: đã bán, -1: bị từ chối

    [ForeignKey("CategoryId")]
    public CategoryEntity? Category { get; set; }
    [ForeignKey("SellerId")]
    public UserEntity? Seller { get; set; }
}