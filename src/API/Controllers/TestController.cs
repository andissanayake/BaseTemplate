using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("secure")]
    [Authorize]
    public IActionResult GetSecureData()
    {
        return Ok(new { Message = "You are authenticated!" });
    }
}
