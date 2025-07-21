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
    /// <summary>
    /// Create a new item attribute.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Creates a new item attribute for the current tenant</li>
    ///   <li>Validates that the item attribute type exists and belongs to the current tenant</li>
    ///   <li>Validates that the name and code are unique within the item attribute type</li>
    ///   <li>Automatically sets IsActive to true and associates with current tenant</li>
    ///   <li>Requires AttributeManager role permission</li>
    /// </ul>
    /// <b>Validation Rules:</b>
    /// <ul>
    ///   <li>Name is required and must be unique within the attribute type</li>
    ///   <li>Code is required and must be unique within the attribute type</li>
    ///   <li>Value is required</li>
    ///   <li>ItemAttributeTypeId must reference an existing attribute type</li>
    ///   <li>Maximum name length: 255 characters</li>
    ///   <li>Maximum code length: 50 characters</li>
    ///   <li>Maximum value length: 500 characters</li>
    /// </ul>
    /// <b>Request body:</b>
    /// <ul>
    ///   <li><c>Name</c> (string, required): Name of the attribute (must be unique within attribute type)</li>
    ///   <li><c>Code</c> (string, required): Code of the attribute (must be unique within attribute type)</li>
    ///   <li><c>Value</c> (string, required): Value of the attribute</li>
    ///   <li><c>ItemAttributeTypeId</c> (int, required): ID of the attribute type this attribute belongs to</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>int</c>: The ID of the newly created item attribute</li>
    /// </ul>
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateItemAttributeCommand command)
    {
        return await SendAsync(command);
    }

    /// <summary>
    /// Update an existing item attribute.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Updates an existing item attribute for the current tenant</li>
    ///   <li>Validates that the attribute exists and belongs to the current tenant</li>
    ///   <li>Validates that the new name and code are unique within the attribute type (if being changed)</li>
    ///   <li>Requires AttributeManager role permission</li>
    /// </ul>
    /// <b>Validation Rules:</b>
    /// <ul>
    ///   <li>Attribute must exist and belong to current tenant</li>
    ///   <li>Name is required and must be unique within the attribute type</li>
    ///   <li>Code is required and must be unique within the attribute type</li>
    ///   <li>Value is required</li>
    ///   <li>Maximum name length: 255 characters</li>
    ///   <li>Maximum code length: 50 characters</li>
    ///   <li>Maximum value length: 500 characters</li>
    /// </ul>
    /// <b>Request body:</b>
    /// <ul>
    ///   <li><c>Id</c> (int, required): ID of the attribute to update</li>
    ///   <li><c>Name</c> (string, required): Updated name of the attribute (must be unique within attribute type)</li>
    ///   <li><c>Code</c> (string, required): Updated code of the attribute (must be unique within attribute type)</li>
    ///   <li><c>Value</c> (string, required): Updated value of the attribute</li>
    ///   <li><c>ItemAttributeTypeId</c> (int, required): ID of the attribute type this attribute belongs to</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>bool</c>: Indicates success or failure</li>
    /// </ul>
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ActionResult<Result<bool>>> Update(int id, [FromBody] UpdateItemAttributeCommand command)
    {
        return await SendAsync(command);
    }

    /// <summary>
    /// Soft delete an item attribute.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Performs a soft delete by setting IsActive to false</li>
    ///   <li>Validates that the attribute exists and belongs to the current tenant</li>
    ///   <li>Does not physically delete the record (preserves data integrity)</li>
    ///   <li>Requires AttributeManager role permission</li>
    /// </ul>
    /// <b>Important Notes:</b>
    /// <ul>
    ///   <li>This is a soft delete - the record is not physically removed</li>
    ///   <li>Associated items will still reference this attribute</li>
    ///   <li>Consider the impact on existing items before deletion</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>bool</c>: Indicates success or failure</li>
    /// </ul>
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<bool>>> Delete(int id)
    {
        return await SendAsync(new DeleteItemAttributeCommand(id));
    }

    /// <summary>
    /// Get a specific item attribute by ID.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Retrieves detailed information about a specific item attribute</li>
    ///   <li>Validates that the attribute belongs to the current user's tenant</li>
    ///   <li>Returns NotFound if the item attribute does not exist or doesn't belong to the tenant</li>
    ///   <li>Requires AttributeManager role permission</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>ItemAttributeDto</c>: Detailed item attribute information including Id, Name, Code, Value, IsActive, ItemAttributeTypeId, ItemAttributeTypeName</li>
    /// </ul>
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ActionResult<Result<ItemAttributeDto>>> GetById(int id)
    {
        return await SendAsync(new GetItemAttributeByIdQuery(id));
    }

    /// <summary>
    /// Get all item attributes for a specific attribute type.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Retrieves all item attributes belonging to a specific attribute type</li>
    ///   <li>Validates that the attribute type belongs to the current user's tenant</li>
    ///   <li>Returns only active attributes (where IsActive = true)</li>
    ///   <li>Results are ordered by creation date (newest first)</li>
    ///   <li>Requires AttributeManager role permission</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>List&lt;ItemAttributeBriefDto&gt;</c>: List of item attributes with basic information including Id, Name, Code, Value, IsActive, ItemAttributeTypeId, ItemAttributeTypeName</li>
    /// </ul>
    /// </remarks>
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
