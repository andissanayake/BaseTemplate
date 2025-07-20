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
    [HttpPost("item-attributes")]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateItemAttributeCommand command)
    {
        if (command == null)
        {
            return BadRequest(Result<int>.Validation("Command is required", []));
        }

        return await SendAsync(command);
    }

    [HttpPut("item-attributes/{id}")]
    public async Task<ActionResult<Result<bool>>> Update(int id, [FromBody] UpdateItemAttributeCommand command)
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

    [HttpDelete("item-attributes/{id}")]
    public async Task<ActionResult<Result<bool>>> Delete(int id)
    {
        return await SendAsync(new DeleteItemAttributeCommand(id));
    }

    [HttpGet("item-attributes/{id}")]
    public async Task<ActionResult<Result<ItemAttributeDto>>> GetById(int id)
    {
        return await SendAsync(new GetItemAttributeByIdQuery(id));
    }

    [HttpGet("item-attributes")]
    public async Task<ActionResult<Result<List<ItemAttributeBriefDto>>>> GetAll(
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int? itemAttributeTypeId = null)
    {
        var query = new GetItemAttributesQuery
        {
            SearchTerm = searchTerm,
            IsActive = isActive,
            ItemAttributeTypeId = itemAttributeTypeId
        };

        return await SendAsync(query);
    }
} 