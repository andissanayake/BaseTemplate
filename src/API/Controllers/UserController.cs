using BaseTemplate.Application.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;
[Authorize]
public class UserController : ApiControllerBase
{
    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<string>>> GetUserRoles()
    {
        var roles = await Mediator.Send(new GetUserRolesQuery());
        return Ok(roles);
    }
}
