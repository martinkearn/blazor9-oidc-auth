using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blazor9OIDC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SecureApiController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("You are authorized to access SecureApiController.");
        }
    }
}
