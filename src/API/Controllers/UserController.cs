using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.GlobalFeatures.Staff.Commands.RespondToStaffInvitation;
using BaseTemplate.Application.GlobalFeatures.Users.Commands.GetUser;
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
    ///   <li><b>Staff Invitation:</b> If the user is not associated with a tenant but has a pending staff invitation, returns details about the invitation, including requester info, requested roles, status (Pending, Accepted, Rejected, Revoked, Expired), creation date, and tenant name.</li>
    /// </ul>
    /// <p>This endpoint is typically used by the frontend to determine the user's identity, permissions, and onboarding state.</p>
    /// </remarks>
    /// <returns>
    /// <p>A <see cref="Result{GetUserResponse}"/> containing:</p>
    /// <ul>
    ///   <li>User roles</li>
    ///   <li>Tenant details (if any)</li>
    ///   <li>Staff invitation information (if any)</li>
    /// </ul>
    /// </returns>
    [HttpPost("userDetails")]
    public async Task<ActionResult<Result<GetUserResponse>>> GetUser()
    {
        return await SendAsync(new GetUserCommand());
    }
    /// <summary>
    /// Respond to a staff invitation (accept or reject) by the invited user.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Allows the invited user to accept or reject a pending staff invitation.</li>
    ///   <li>When accepting: Updates invitation status to accepted, sets acceptance timestamp, updates user's tenant association, and assigns the requested roles to the user.</li>
    ///   <li>When rejecting: Updates invitation status to rejected and stores the rejection reason.</li>
    ///   <li>Requires rejection reason when rejecting the invitation.</li>
    ///   <li>Only works on invitations that are still in pending status.</li>
    ///   <li>Invalidates user profile cache after successful acceptance.</li>
    /// </ul>
    /// <b>Request body:</b>
    /// <ul>
    ///   <li><c>StaffInvitationId</c> (int, required): ID of the staff invitation to respond to</li>
    ///   <li><c>IsAccepted</c> (bool, required): Whether to accept or reject the invitation</li>
    ///   <li><c>RejectionReason</c> (string, optional): Reason for rejection (required when rejecting)</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>bool</c>: Indicates success or failure</li>
    /// </ul>
    /// </remarks>
    [HttpPost("staff-invitations/respond")]
    public async Task<ActionResult<Result<bool>>> RespondToStaffInvitation(RespondToStaffInvitationCommand command)
    {
        return await SendAsync(command);
    }
}
