
namespace HandHubAPI.Domain.Entities;

using System.ComponentModel.DataAnnotations.Schema;

public class NotificationEntity : BaseEntity
{
    public int ReceiverId { get; set; }
    public int SenderId { get; set; }
    public int Type { get; set; } // 1:negotiation, 2: chat
    public int RelatedId { get; set; } // ID of the related entity (e.g., PriceNegotiationId, ChatId)
    public int ProductId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    [ForeignKey("ReceiverId")]
    public UserEntity? Receiver { get; set; }
    [ForeignKey("SenderId")]
    public UserEntity? Sender { get; set; }
}