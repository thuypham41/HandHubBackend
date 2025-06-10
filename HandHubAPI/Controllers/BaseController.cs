using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HandHubAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController<T> : ControllerBase where T : BaseController<T>
    {
        private readonly ILogger<T> _logger;

        protected BaseController(ILogger<T> logger)
        {
            _logger = logger;
        }

        protected IActionResult CommonResponse<TData>(
            TData data,
            string message = null,
            bool success = true,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            try
            {
                var response = new ApiResponse<TData>
                {
                    Success = success,
                    Message = message,
                    Data = data,
                    Timestamp = DateTime.UtcNow
                };

                _logger.LogInformation("Request processed successfully: {RequestId}", HttpContext.TraceIdentifier);
                return StatusCode((int)statusCode, response);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        protected IActionResult PaginatedResponse<TData>(
            IEnumerable<TData> data,
            int page,
            int pageSize,
            int totalCount,
            string message = null)
        {
            try
            {
                var response = new PaginatedApiResponse<TData>
                {
                    Success = true,
                    Message = message,
                    Data = data,
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    },
                    Timestamp = DateTime.UtcNow,
                };

                _logger.LogInformation("Paginated request processed: {RequestId}, Page: {Page}, Total: {Total}", HttpContext.TraceIdentifier, page, totalCount);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        protected IActionResult ErrorResponse(
            string message,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            Exception ex = null)
        {
            var response = new ApiResponse<object>
            {
                Success = false,
                Message = message,
                ErrorCode = statusCode.ToString(),
                Timestamp = DateTime.UtcNow
            };

            _logger.LogError(ex, "Error occurred: {Message}, RequestId: {RequestId}",
                message, HttpContext.TraceIdentifier);
            return StatusCode((int)statusCode, response);
        }

        private IActionResult HandleError(Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred: {RequestId}", HttpContext.TraceIdentifier);

            return ErrorResponse(
                message: "An unexpected error occurred",
                statusCode: HttpStatusCode.InternalServerError,
                ex: ex);
        }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public string? ErrorCode { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class PaginatedApiResponse<T> : ApiResponse<IEnumerable<T>>
    {
        public PaginationInfo? Pagination { get; set; }
    }

    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}