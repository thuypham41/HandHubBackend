using Microsoft.AspNetCore.SignalR;

namespace HandHubAPI.Hubs;
// Class này sẽ giúp SignalR biết cách lấy User ID từ mỗi kết nối
public class CustomUserIdProvider : IUserIdProvider
{
    public virtual string? GetUserId(HubConnectionContext connection)
    {
        // Lấy userId từ query string mà client đã gửi lên trong URL
        return connection.GetHttpContext()?.Request.Query["userId"];
    }
}