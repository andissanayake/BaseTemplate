using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.TenantFeatures.Items.Commands.CreateItem;
using BaseTemplate.Application.TenantFeatures.Items.Commands.DeleteItem;
using BaseTemplate.Application.TenantFeatures.Items.Commands.UpdateItem;
using BaseTemplate.Application.TenantFeatures.Items.Commands.UpdateItemCharacteristicType;
using BaseTemplate.Application.TenantFeatures.Items.Queries.GetItemById;
using BaseTemplate.Application.TenantFeatures.Items.Queries.GetItemsWithPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[Authorize]
[Route("api/item")]
public class ItemController : ApiControllerBase
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
        return await SendAsync(command);
    }

    [HttpPut]
    public async Task<ActionResult<Result<bool>>> Update(UpdateItemCommand command)
    {
        return await SendAsync(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<bool>>> Delete(int id)
    {
        return await SendAsync(new DeleteItemCommand(id));
    }

    [HttpPut("characteristic-type")]
    public async Task<ActionResult<Result<bool>>> UpdateCharacteristicTypes(UpdateItemCharacteristicTypeCommand command)
    {
        return await SendAsync(command);
    }
}
