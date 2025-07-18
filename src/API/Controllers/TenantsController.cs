using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Staff.Commands.RemoveStaff;
using BaseTemplate.Application.Staff.Commands.RequestStaff;
using BaseTemplate.Application.Staff.Commands.RespondToStaffRequest;
using BaseTemplate.Application.Staff.Commands.UpdateStaffRequest;
using BaseTemplate.Application.Staff.Commands.UpdateStaffRoles;
using BaseTemplate.Application.Staff.Queries.GetStaffMember;
using BaseTemplate.Application.Staff.Queries.GetStaffRequests;
using BaseTemplate.Application.Staff.Queries.ListStaff;
using BaseTemplate.Application.Tenants.Commands.CreateTenant;
using BaseTemplate.Application.Tenants.Commands.UpdateTenant;
using BaseTemplate.Application.Tenants.Queries.GetTenantById;
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
        return await SendAsync(new GetTenantByIdQuery());
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
    ///   <li>Only users with the <c>TenantManager</c> or <c>TenantOwner</c> role can perform this action.</li>
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

    [HttpPost("{tenantId}/request-staff")]
    public async Task<ActionResult<Result<bool>>> RequestStaff(int tenantId, RequestStaffCommand command)
    {
        // No need to check tenantId in body; tenantId is only in the route now
        return await SendAsync(command);
    }

    [HttpGet("{tenantId}/staff-requests")]
    public async Task<ActionResult<Result<List<StaffRequestDto>>>> GetStaffRequests(int tenantId)
    {
        return await SendAsync(new GetStaffRequestsQuery(tenantId));
    }

    [HttpPost("staff-requests/{staffRequestId}/update")]
    public async Task<ActionResult<Result<bool>>> UpdateStaffRequest(int staffRequestId, UpdateStaffRequestCommand command)
    {

        return await SendAsync(command);
    }

    [HttpPost("staff-requests/{staffRequestId}/respond")]
    public async Task<ActionResult<Result<bool>>> RespondToStaffRequest(int staffRequestId, RespondToStaffRequestCommand command)
    {
        if (staffRequestId != command.StaffRequestId)
        {
            return BadRequest();
        }

        return await SendAsync(command);
    }

    // Staff Management Endpoints
    [HttpGet("{tenantId}/staff")]
    public async Task<ActionResult<Result<List<StaffMemberDto>>>> ListStaff(int tenantId)
    {
        return await SendAsync(new ListStaffQuery(tenantId));
    }

    [HttpGet("{tenantId}/staff/{staffId}")]
    public async Task<ActionResult<Result<StaffMemberDetailDto>>> GetStaffMember(int tenantId, int staffId)
    {
        return await SendAsync(new GetStaffMemberQuery(tenantId, staffId));
    }

    [HttpDelete("{tenantId}/staff/{staffId}")]
    public async Task<ActionResult<Result<bool>>> RemoveStaff(int tenantId, int staffId)
    {
        return await SendAsync(new RemoveStaffCommand(tenantId, staffId));
    }

    [HttpPut("{tenantId}/staff/{staffId}/roles")]
    public async Task<ActionResult<Result<bool>>> UpdateStaffRoles(int tenantId, int staffId, UpdateStaffRolesCommand command)
    {
        if (tenantId != command.TenantId || staffId != command.StaffId)
        {
            return BadRequest();
        }

        return await SendAsync(command);
    }
}
