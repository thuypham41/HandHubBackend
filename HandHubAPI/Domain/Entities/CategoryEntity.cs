namespace HandHubAPI.Domain.Entities;
using System.ComponentModel.DataAnnotations;

public class CategoryEntity : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}