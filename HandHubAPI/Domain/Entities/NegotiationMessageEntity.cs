using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandHubAPI.Domain.Entities;

public class NegotiationMessageEntity : BaseEntity
{
    public int PriceNegotiationId { get; set; }

    public int SenderId { get; set; }
    public int ReceivierId { get; set; }
    public string MessageContent { get; set; } = string.Empty;
    [ForeignKey("PriceNegotiationId")]
    public PriceNegotiationEntity? PriceNegotiation { get; set; }
    [ForeignKey("SenderId")]
    public UserEntity? Sender { get; set; }
    [ForeignKey("ReceivierId")]
    public UserEntity? Receivier{ get; set; }
}