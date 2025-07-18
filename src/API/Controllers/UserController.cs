using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Users.Queries.GetUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;
[Authorize]
public class UserController : ApiControllerBase
{
    /// <summary>
    /// Returns identity and context information for the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// <p>This endpoint provides a comprehensive profile of the authenticated user, including:</p>
    /// <ul>
    ///   <li><b>User Record:</b> Ensures the user exists in the system, creating a new record if necessary, and updates name/email if changed.</li>
    ///   <li><b>Roles:</b> Lists all roles assigned to the user. If the user is a Tenant Owner, all base tenant roles are included.</li>
    ///   <li><b>Tenant Information:</b> If the user is associated with a tenant, returns the tenant's ID and name.</li>
    ///   <li><b>Staff Request:</b> If the user is not associated with a tenant but has a pending staff request (invitation), returns details about the request, including requester info, requested roles, status (Pending, Accepted, Rejected, Revoked, Expired), creation date, and tenant name.</li>
    /// </ul>
    /// <p>This endpoint is typically used by the frontend to determine the user's identity, permissions, and onboarding state.</p>
    /// </remarks>
    /// <returns>
    /// <p>A <see cref="Result{GetUserResponse}"/> containing:</p>
    /// <ul>
    ///   <li>User roles</li>
    ///   <li>Tenant details (if any)</li>
    ///   <li>Staff request information (if any)</li>
    /// </ul>
    /// </returns>
    [HttpPost("userDetails")]
    public async Task<ActionResult<Result<GetUserResponse>>> GetUser()
    {
        return await SendAsync(new GetUserQuery());
    }
}
