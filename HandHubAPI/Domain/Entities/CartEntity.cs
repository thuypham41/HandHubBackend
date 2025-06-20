using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandHubAPI.Domain.Entities;

public class CartEntity : BaseEntity
{
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public UserEntity? User { get; set; }
    public decimal TotalPrice { get; set; }
    public int Status { get; set; }
}