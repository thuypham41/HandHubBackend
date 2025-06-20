using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandHubAPI.Domain.Entities;

public class CartItemEntity : BaseEntity
{
    public int CartId { get; set; }

    [ForeignKey("CartId")]
    public CartEntity? Cart { get; set; }
    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    public ProductEntity? Product { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }
}