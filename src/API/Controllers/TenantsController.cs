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
    [HttpGet("{tenantId}")]
    public async Task<ActionResult<Result<GetTenantResponse>>> GetById(int tenantId)
    {
        return await SendAsync(new GetTenantByIdQuery(tenantId));
    }

    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create(CreateTenantCommand command)
    {
        return await SendAsync(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Result<bool>>> Update(int id, UpdateTenantCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        return await SendAsync(command);
    }

    [HttpPost("{tenantId}/request-staff")]
    public async Task<ActionResult<Result<bool>>> RequestStaff(int tenantId, RequestStaffCommand command)
    {
        if (tenantId != command.TenantId)
        {
            return BadRequest();
        }

        return await SendAsync(command);
    }

    [HttpGet("{tenantId}/staff-requests")]
    public async Task<ActionResult<Result<List<StaffRequestDto>>>> GetStaffRequests(int tenantId)
    {
        return await SendAsync(new GetStaffRequestsQuery(tenantId));
    }

    [HttpPost("{tenantId}/staff-requests/{staffRequestId}/update")]
    public async Task<ActionResult<Result<bool>>> UpdateStaffRequest(int tenantId, int staffRequestId, UpdateStaffRequestCommand command)
    {
        if (tenantId != command.TenantId || staffRequestId != command.StaffRequestId)
        {
            return BadRequest();
        }

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
