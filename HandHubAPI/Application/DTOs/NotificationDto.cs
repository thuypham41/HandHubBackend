public class NotificationDto
{
    public int Id { get; set; }
    public string Messeage { get; set; } = string.Empty;
    public int ReceiverId { get; set; }
    public int SenderId { get; set; }
    public bool IsRead { get; set; } = false;
    public int Type { get; set; } // 1:negotiation, 2: chat
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
}