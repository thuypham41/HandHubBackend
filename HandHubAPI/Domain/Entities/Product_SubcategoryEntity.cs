namespace HandHubAPI.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

public class Product_SubcategoryEntity : BaseEntity
{
    public int ProductId { get; set; }

    public int SubcategoryId { get; set; }

    [ForeignKey("SubcategoryId")]
    public SubCategoryEntity? SubCategory { get; set; }

    [ForeignKey("ProductId")]
    public ProductEntity? Product { get; set; }
}