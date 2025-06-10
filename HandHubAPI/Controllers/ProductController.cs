using Microsoft.AspNetCore.Mvc;
namespace HandHubAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : BaseController<ProductController>
{
    public ProductController(ILogger<ProductController> logger) : base(logger)
    {
    }
}