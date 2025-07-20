using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.ItemAttributes.Commands.CreateItemAttribute;
using BaseTemplate.Application.ItemAttributes.Commands.DeleteItemAttribute;
using BaseTemplate.Application.ItemAttributes.Commands.UpdateItemAttribute;
using BaseTemplate.Application.ItemAttributes.Queries.GetItemAttributeById;
using BaseTemplate.Application.ItemAttributes.Queries.GetItemAttributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[Authorize]
public class ItemAttributesController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateItemAttributeCommand command)
    {
        return await SendAsync(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Result<bool>>> Update(int id, [FromBody] UpdateItemAttributeCommand command)
    {
        return await SendAsync(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<bool>>> Delete(int id)
    {
        return await SendAsync(new DeleteItemAttributeCommand(id));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<ItemAttributeDto>>> GetById(int id)
    {
        return await SendAsync(new GetItemAttributeByIdQuery(id));
    }

    [HttpGet("/api/itemAttributeType/{itemAttributeTypeId}/ItemAttribute")]
    public async Task<ActionResult<Result<List<ItemAttributeBriefDto>>>> GetAll(
        [FromQuery] int itemAttributeTypeId)
    {
        var query = new GetItemAttributesQuery
        {
            ItemAttributeTypeId = itemAttributeTypeId
        };

        return await SendAsync(query);
    }
}
