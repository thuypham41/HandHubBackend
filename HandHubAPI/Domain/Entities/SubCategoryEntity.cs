namespace HandHubAPI.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

public class SubCategoryEntity : BaseEntity
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;

    [ForeignKey("CategoryId")]
    public CategoryEntity? Category { get; set; }
}