using BaseTemplate.Application.Common.Models;
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
}
