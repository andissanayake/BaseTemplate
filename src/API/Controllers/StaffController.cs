using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.TenantFeatures.Staff.Commands.RemoveStaff;
using BaseTemplate.Application.TenantFeatures.Staff.Commands.UpdateStaffRoles;
using BaseTemplate.Application.TenantFeatures.Staff.Queries.GetStaffMember;
using BaseTemplate.Application.TenantFeatures.Staff.Queries.ListStaff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[Authorize]
[Route("api/staff")]
public class StaffController : ApiControllerBase
{
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
    [HttpGet]
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
    [HttpGet("{staffId}")]
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
    [HttpDelete("{staffId}")]
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
    [HttpPut("roles")]
    public async Task<ActionResult<Result<bool>>> UpdateStaffRoles(UpdateStaffRolesCommand command)
    {
        return await SendAsync(command);
    }
}
