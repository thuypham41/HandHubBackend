using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandHubAPI.Domain.Entities;

public class OrderEntity : BaseEntity
{
    public int BuyerId { get; set; }

    public int Price { get; set; }

    [MaxLength(100)]
    public string PaymentMethod { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Address { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; }

    public int Status { get; set; } // 0: Đang xử lý, 1: Đang giao hàng, 2: Đã giao hàng, 3: Tất cả

    public int TotalMoney { get; set; }
    [ForeignKey("BuyerId")]
    public UserEntity? Buyer { get; set; }
}
