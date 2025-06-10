
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandHubAPI.Domain.Entities;

public class UserEntity : BaseEntity
{
    [MaxLength(50)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Email { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    public int RoleId { get; set; }
    [ForeignKey("RoleId")]
    public RoleEntity? Role{ get; set; }
}
