using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;
[Authorize]
public class UserController : ApiControllerBase
{
    [HttpPost("userDetails")]
    public async Task<ActionResult<Result<GetUserResponse>>> GetUser()
    {
        return await SendAsync(new GetUserQuery());
    }
}
