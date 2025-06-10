using System.ComponentModel.DataAnnotations;

namespace HandHubAPI.Domain.Entities;
public class RoleEntity : BaseEntity
{
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;
}