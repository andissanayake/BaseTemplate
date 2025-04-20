using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;
[Authorize]
public class UserController : ApiControllerBase
{
    [HttpGet("roles")]
    public async Task<ActionResult<Result<IEnumerable<string>>>> GetUserRoles()
    {
        return await SendAsync(new GetUserRolesQuery());
    }
}
