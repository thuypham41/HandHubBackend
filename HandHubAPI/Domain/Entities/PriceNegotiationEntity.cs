using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandHubAPI.Domain.Entities;

public class PriceNegotiationEntity : BaseEntity
{
    public int ProductId { get; set; }

    public int BuyerId { get; set; }

    public int OfferPrice { get; set; }

    public int Status { get; set; }

    [MaxLength(50)]
    public string SellerResponse { get; set; } = string.Empty;

    public int FinalPrice { get; set; }
    [ForeignKey("ProductId")]
    public ProductEntity? Product { get; set; }
    [ForeignKey("BuyerId")]
    public UserEntity? Buyer{ get; set; }
}
