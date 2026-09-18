using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Api.Controllers;

[Route("api/[controller]")]
public class HealthController : ApiController
{
    public HealthController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            service = "MTK API"
        });
    }
}
