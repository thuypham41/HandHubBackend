namespace HandHubAPI.Application.DTOs;

public class SendMessageRequestDto
{
    public int? MessageId { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime? SendTime { get; set; } = DateTime.UtcNow;
}