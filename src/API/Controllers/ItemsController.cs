using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Items.Commands.CreateItem;
using BaseTemplate.Application.Items.Commands.DeleteItem;
using BaseTemplate.Application.Items.Commands.UpdateItem;
using BaseTemplate.Application.Items.Queries.GetItemById;
using BaseTemplate.Application.Items.Queries.GetItemsWithPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[Authorize]
[Route("api")]
public class ItemsController : ApiControllerBase
{
    [HttpGet("tenants/{tenantId}/items")]
    public async Task<ActionResult<Result<PaginatedList<ItemBriefDto>>>> GetItemsWithPagination([FromQuery] GetItemsWithPaginationQuery query)
    {
        return await SendAsync(query);
    }

    [HttpGet("tenants/{tenantId}/items/{id}")]
    public async Task<ActionResult<Result<ItemDto>>> GetById(int tenantId, int id)
    {
        return await SendAsync(new GetItemByIdQuery(tenantId, id));
    }

    [HttpPost("tenants/{tenantId}/items")]
    public async Task<ActionResult<Result<int>>> Create(int tenantId, CreateItemCommand command)
    {
        if (command == null)
        {
            return BadRequest(Result<int>.Validation("Command is required"));
        }

        if (tenantId != command.TenantId)
        {
            return BadRequest(Result<int>.Validation("Tenant ID mismatch"));
        }
        
        return await SendAsync(command);
    }

    [HttpPut("tenants/{tenantId}/items/{id}")]
    public async Task<ActionResult<Result<bool>>> Update(int tenantId, int id, UpdateItemCommand command)
    {
        if (command == null)
        {
            return BadRequest(Result<bool>.Validation("Command is required"));
        }

        if (id != command.Id || tenantId != command.TenantId)
        {
            return BadRequest(Result<bool>.Validation("ID or Tenant ID mismatch"));
        }

        return await SendAsync(command);
    }

    [HttpDelete("tenants/{tenantId}/items/{id}")]
    public async Task<ActionResult<Result<bool>>> Delete(int tenantId, int id)
    {
        return await SendAsync(new DeleteItemCommand(tenantId, id));
    }
}
