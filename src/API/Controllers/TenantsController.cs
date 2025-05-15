using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.TenantLists.Commands.GetTenantListById;
using BaseTemplate.Application.Tenants.Commands.CreateTenant;
using BaseTemplate.Application.Tenants.Commands.UpdateTenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[Authorize]
public class TenantsController : ApiControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<Result<GetTenantResponce>>> GetById(int id)
    {
        return await SendAsync(new GetTenantByIdQuery(id));
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
