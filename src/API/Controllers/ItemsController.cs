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
public class ItemsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Result<PaginatedList<ItemBriefDto>>>> GetItemsWithPagination([FromQuery] GetItemsWithPaginationQuery query)
    {
        return await SendAsync(query);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<ItemDto>>> GetById(int id)
    {
        return await SendAsync(new GetItemByIdQuery(id));
    }

    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create(CreateItemCommand command)
    {
        if (command == null)
        {
            return BadRequest(Result<int>.Validation("Command is required", []));
        }

        return await SendAsync(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Result<bool>>> Update(int id, UpdateItemCommand command)
    {
        if (command == null)
        {
            return BadRequest(Result<bool>.Validation("Command is required", []));
        }

        if (id != command.Id)
        {
            return BadRequest(Result<bool>.Validation("ID mismatch", []));
        }

        return await SendAsync(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<bool>>> Delete(int id)
    {
        return await SendAsync(new DeleteItemCommand(id));
    }
}
