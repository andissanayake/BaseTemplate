using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.GlobalFeatures.Staff.Commands.RespondToStaffInvitation;
using BaseTemplate.Application.TenantFeatures.Staff.Commands.CreateStaffInvitation;
using BaseTemplate.Application.TenantFeatures.Staff.Commands.RevokeStaffInvitation;
using BaseTemplate.Application.TenantFeatures.Staff.Queries.GetStaffInvitation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[Authorize]
[Route("api/staff-invitation")]
public class StaffInvitationController : ApiControllerBase
{
    /// <summary>
    /// Request to add a new staff member to the current user's tenant.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Creates a staff invitation for the specified email address and name.</li>
    ///   <li>Validates that only allowed tenant base roles can be requested.</li>
    ///   <li>Checks if the user already exists in the system and belongs to another tenant.</li>
    ///   <li>Prevents duplicate pending invitations for the same email in the tenant.</li>
    ///   <li>Creates a staff invitation record with pending status.</li>
    ///   <li>Associates the requested roles with the staff invitation.</li>
    ///   <li>Only users with the <c>StaffManager</c> role can perform this action.</li>
    /// </ul>
    /// <b>Request body:</b>
    /// <ul>
    ///   <li><c>StaffEmail</c> (string, required): Email address of the staff member (must be valid email format)</li>
    ///   <li><c>StaffName</c> (string, required): Name of the staff member (2-100 characters)</li>
    ///   <li><c>Roles</c> (List&lt;string&gt;, required): List of roles to assign (must be from allowed tenant base roles)</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>bool</c>: Indicates success or failure</li>
    /// </ul>
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<Result<bool>>> CreateStaffInvitation(CreateStaffInvitationCommand command)
    {
        // No need to check tenantId in body; tenantId is only in the route now
        return await SendAsync(command);
    }

    /// <summary>
    /// Get all staff invitations for the current user's tenant.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Retrieves all staff invitations associated with the current user's tenant.</li>
    ///   <li>Orders invitations by creation date (newest first).</li>
    ///   <li>Includes all associated roles for each staff invitation.</li>
    ///   <li>Returns invitations in all statuses (pending, accepted, rejected, revoked).</li>
    ///   <li>Includes requester information and timestamps.</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>List&lt;StaffInvitationDto&gt;</c>: List of staff invitations with details including ID, email, name, roles, status, timestamps, and rejection reasons</li>
    /// </ul>
    /// </remarks>
    [HttpGet]
    public async Task<ActionResult<Result<List<StaffInvitationDto>>>> GetStaffInvitations()
    {
        return await SendAsync(new GetStaffInvitationsQuery());
    }

    /// <summary>
    /// Update (revoke/reject) a staff invitation by the tenant owner.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Updates the status of a pending staff invitation to revoked.</li>
    ///   <li>Only works on invitations that are still in pending status.</li>
    ///   <li>Requires a rejection reason to be provided.</li>
    ///   <li>Only users with the <c>StaffInvitationManager</c> role can perform this action.</li>
    ///   <li>Fails if the staff invitation has already been processed (accepted/rejected).</li>
    /// </ul>
    /// <b>Request body:</b>
    /// <ul>
    ///   <li><c>StaffInvitationId</c> (int, required): ID of the staff invitation to update</li>
    ///   <li><c>RejectionReason</c> (string, required): Reason for rejecting the staff invitation</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>bool</c>: Indicates success or failure</li>
    /// </ul>
    /// </remarks>
    [HttpPost("revoke")]
    public async Task<ActionResult<Result<bool>>> RevokeStaffInvitation(RevokeStaffInvitationCommand command) => await SendAsync(command);

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
    [HttpPost("respond")]
    public async Task<ActionResult<Result<bool>>> RespondToStaffInvitation(RespondToStaffInvitationCommand command)
    {
        return await SendAsync(command);
    }
}
