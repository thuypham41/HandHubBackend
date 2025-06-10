using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandHubAPI.Domain.Entities;

public class ProductApprovalEntity : BaseEntity
{
    public int ProductId { get; set; }

    public int AdminId { get; set; }
    public int Status { get; set; }
    [ForeignKey("ProductId")]
    public ProductEntity? Product { get; set; }
    [ForeignKey("AdminId")]
    public UserEntity? Admin{ get; set; }
}
