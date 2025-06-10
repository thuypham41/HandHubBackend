using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandHubAPI.Domain.Entities;

public class OrderDetailEntity : BaseEntity
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }

    public int Price { get; set; }

    public int Num { get; set; }

    public int TotalMoney { get; set; }
    [ForeignKey("ProductId")]
    public ProductEntity? Product { get; set; }
    [ForeignKey("OrderId")]
    public OrderEntity? Order{ get; set; }
}
