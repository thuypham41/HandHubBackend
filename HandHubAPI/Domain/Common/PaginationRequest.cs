namespace HandHubAPI.Domain.Common;

public class PaginationRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 30;
    public int ValidPageNumber => Math.Max(1, PageNumber);
    public int ValidPageSize => Math.Max(1, Math.Min(100, PageSize));
}