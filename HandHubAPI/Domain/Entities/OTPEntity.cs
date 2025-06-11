namespace HandHubAPI.Domain.Entities;

public class OTPEntity : BaseEntity
{
    public string? Email { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}