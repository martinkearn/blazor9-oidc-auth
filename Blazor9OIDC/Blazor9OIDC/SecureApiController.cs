using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blazor9OIDC
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecureApiController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("You are authorized to access this secure API endpoint.");
        }
    }
}
