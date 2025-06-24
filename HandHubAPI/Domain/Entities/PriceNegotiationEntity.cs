using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandHubAPI.Domain.Entities;

public class PriceNegotiationEntity : BaseEntity
{
    public int ProductId { get; set; }

    public int BuyerId { get; set; }

    public decimal OfferPrice { get; set; }

    public int Status { get; set; } // 0: Wait, 1: Accepted, 2: Rejected

    [MaxLength(50)]
    public string SellerResponse { get; set; } = string.Empty;

    public decimal FinalPrice { get; set; }
    [ForeignKey("ProductId")]
    public ProductEntity? Product { get; set; }
    [ForeignKey("BuyerId")]
    public UserEntity? Buyer{ get; set; }
}
