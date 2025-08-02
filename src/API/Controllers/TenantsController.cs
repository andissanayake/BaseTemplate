using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.GlobalFeatures.Tenants.Commands.CreateTenant;
using BaseTemplate.Application.TenantFeatures.Staff.Commands.CreateStaffInvitation;
using BaseTemplate.Application.TenantFeatures.Staff.Commands.RemoveStaff;
using BaseTemplate.Application.TenantFeatures.Staff.Commands.RevokeStaffInvitation;
using BaseTemplate.Application.TenantFeatures.Staff.Commands.UpdateStaffRoles;
using BaseTemplate.Application.TenantFeatures.Staff.Queries.GetStaffInvitation;
using BaseTemplate.Application.TenantFeatures.Staff.Queries.GetStaffMember;
using BaseTemplate.Application.TenantFeatures.Staff.Queries.ListStaff;
using BaseTemplate.Application.TenantFeatures.Tenants.Commands.UpdateTenant;
using BaseTemplate.Application.TenantFeatures.Tenants.Queries.GetTenantById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[Authorize]
public class TenantsController : ApiControllerBase
{
    /// <summary>
    /// Get the current user's tenant details.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Fetches the tenant associated with the current authenticated user.</li>
    ///   <li>If the user does not have a tenant, returns NotFound.</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>Id</c> (int): Tenant ID</li>
    ///   <li><c>Name</c> (string): Tenant name</li>
    ///   <li><c>Address</c> (string, optional): Tenant address</li>
    /// </ul>
    /// </remarks>
    [HttpGet]
    public async Task<ActionResult<Result<GetTenantResponse>>> GetById()
    {
        return await SendAsync(new GetTenantQuery());
    }

    /// <summary>
    /// Create a new tenant and assign the current user as the owner.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Checks if the user already has a tenant. If so, returns the existing tenant ID and does not create a new tenant.</li>
    ///   <li>If not, creates a new tenant with the provided name and address, and sets the current user as the owner.</li>
    ///   <li>Updates the user's tenant association in the database.</li>
    ///   <li>Adds the <c>TenantOwner</c> role to the user.</li>
    ///   <li>Invalidates the user profile cache to ensure fresh data on subsequent requests.</li>
    ///   <li>Any authenticated user can perform this action.</li>
    /// </ul>
    /// <b>Request body:</b>
    /// <ul>
    ///   <li><c>Name</c> (string, required): Tenant name (2-100 characters)</li>
    ///   <li><c>Address</c> (string, optional): Tenant address (max 500 characters)</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>int</c>: The new or existing tenant ID</li>
    /// </ul>
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create(CreateTenantCommand command)
    {
        return await SendAsync(command);
    }

    /// <summary>
    /// Update the details of the current user's tenant.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Updates the name and address of the tenant associated with the current user.</li>
    ///   <li>Only users with the <c>TenantManager</c> role can perform this action.</li>
    ///   <li>Fails with NotFound if the tenant does not exist for the user.</li>
    /// </ul>
    /// <b>Request body:</b>
    /// <ul>
    ///   <li><c>Name</c> (string, required): New tenant name (2-100 characters)</li>
    ///   <li><c>Address</c> (string, optional): New tenant address (max 500 characters)</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>bool</c>: Indicates success or failure</li>
    /// </ul>
    /// </remarks>
    [HttpPut]
    public async Task<ActionResult<Result<bool>>> Update(UpdateTenantCommand command)
    {
        return await SendAsync(command);
    }

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
    [HttpPost("staff-invitation")]
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
    [HttpGet("staff-invitation")]
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
    [HttpPost("staff-invitations/revoke")]
    public async Task<ActionResult<Result<bool>>> RevokeStaffInvitation(RevokeStaffInvitationCommand command) => await SendAsync(command);

    // Staff Management Endpoints
    /// <summary>
    /// Get all staff members for the current user's tenant.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Retrieves all staff members associated with the current user's tenant.</li>
    ///   <li>Excludes the actual tenant owner from the results.</li>
    ///   <li>Orders staff members by creation date (newest first).</li>
    ///   <li>Includes all roles assigned to each staff member.</li>
    ///   <li>Only users with the <c>StaffManager</c> role can perform this action.</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>List&lt;StaffMemberDto&gt;</c>: List of staff members with details including ID, name, email, roles, and timestamps</li>
    /// </ul>
    /// </remarks>
    [HttpGet("staff")]
    public async Task<ActionResult<Result<List<StaffMemberDto>>>> ListStaff()
    {
        return await SendAsync(new ListStaffQuery());
    }

    /// <summary>
    /// Get detailed information about a specific staff member.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Retrieves detailed information about a specific staff member.</li>
    ///   <li>Verifies the staff member belongs to the current user's tenant.</li>
    ///   <li>Includes all roles assigned to the staff member.</li>
    ///   <li>Only users with the <c>StaffManager</c> role can perform this action.</li>
    ///   <li>Fails if the staff member does not exist or does not belong to the tenant.</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>StaffMemberDetailDto</c>: Detailed staff member information including ID, name, email, roles, timestamps, and tenant ID</li>
    /// </ul>
    /// </remarks>
    [HttpGet("staff/{staffId}")]
    public async Task<ActionResult<Result<StaffMemberDetailDto>>> GetStaffMember(int staffId)
    {
        return await SendAsync(new GetStaffMemberQuery(staffId));
    }

    /// <summary>
    /// Remove a staff member from the current user's tenant.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Removes all roles assigned to the staff member.</li>
    ///   <li>Sets the staff member's tenant association to null (removes from tenant).</li>
    ///   <li>Updates the last modified timestamp for the user.</li>
    ///   <li>Verifies the staff member exists and belongs to the current user's tenant.</li>
    ///   <li>Only users with the <c>StaffManager</c> role can perform this action.</li>
    ///   <li>Fails if the staff member does not exist or does not belong to the tenant.</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>bool</c>: Indicates success or failure</li>
    /// </ul>
    /// </remarks>
    [HttpDelete("staff/{staffId}")]
    public async Task<ActionResult<Result<bool>>> RemoveStaff(int staffId)
    {
        return await SendAsync(new RemoveStaffCommand(staffId));
    }

    /// <summary>
    /// Update the roles assigned to a staff member.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Removes all existing roles assigned to the staff member.</li>
    ///   <li>Assigns the new roles specified in the request.</li>
    ///   <li>Verifies the staff member exists and belongs to the current user's tenant.</li>
    ///   <li>Invalidates the user profile cache to ensure fresh data.</li>
    ///   <li>Only users with the <c>StaffManager</c> role can perform this action.</li>
    ///   <li>Fails if the staff member does not exist or does not belong to the tenant.</li>
    /// </ul>
    /// <b>Request body:</b>
    /// <ul>
    ///   <li><c>StaffId</c> (int, required): ID of the staff member to update</li>
    ///   <li><c>NewRoles</c> (List&lt;string&gt;, required): New list of roles to assign to the staff member</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>bool</c>: Indicates success or failure</li>
    /// </ul>
    /// </remarks>
    [HttpPut("staff/roles")]
    public async Task<ActionResult<Result<bool>>> UpdateStaffRoles(UpdateStaffRolesCommand command)
    {

        return await SendAsync(command);
    }
}
