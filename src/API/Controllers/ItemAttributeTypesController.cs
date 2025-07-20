using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.ItemAttributeTypes.Commands.CreateItemAttributeType;
using BaseTemplate.Application.ItemAttributeTypes.Commands.DeleteItemAttributeType;
using BaseTemplate.Application.ItemAttributeTypes.Commands.UpdateItemAttributeType;
using BaseTemplate.Application.ItemAttributeTypes.Queries.GetItemAttributeTypeById;
using BaseTemplate.Application.ItemAttributeTypes.Queries.GetItemAttributeTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[Authorize]
public class ItemAttributeTypesController : ApiControllerBase
{
    /// <summary>
    /// Get all item attribute types with optional filtering.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Retrieves all item attribute types with optional search and filtering.</li>
    ///   <li>Supports searching by name using the searchTerm parameter.</li>
    ///   <li>Supports filtering by active status using the isActive parameter.</li>
    /// </ul>
    /// <b>Query Parameters:</b>
    /// <ul>
    ///   <li><c>searchTerm</c> (string, optional): Search term to filter by name</li>
    ///   <li><c>isActive</c> (bool, optional): Filter by active status</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>List&lt;ItemAttributeTypeBriefDto&gt;</c>: List of item attribute types with basic information</li>
    /// </ul>
    /// </remarks>
    [HttpGet("item-attribute-types")]
    public async Task<ActionResult<Result<List<ItemAttributeTypeBriefDto>>>> GetAll(
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null)
    {
        var query = new GetItemAttributeTypesQuery
        {
            SearchTerm = searchTerm,
            IsActive = isActive
        };

        return await SendAsync(query);
    }

    /// <summary>
    /// Get a specific item attribute type by ID.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Retrieves detailed information about a specific item attribute type.</li>
    ///   <li>Returns NotFound if the item attribute type does not exist.</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>ItemAttributeTypeDto</c>: Detailed item attribute type information</li>
    /// </ul>
    /// </remarks>
    [HttpGet("item-attribute-types/{id}")]
    public async Task<ActionResult<Result<ItemAttributeTypeDto>>> GetById(int id)
    {
        var query = new GetItemAttributeTypeByIdQuery { Id = id };
        return await SendAsync(query);
    }

    /// <summary>
    /// Create a new item attribute type.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Creates a new item attribute type with the provided information.</li>
    ///   <li>Validates the input data and ensures uniqueness.</li>
    /// </ul>
    /// <b>Request body:</b>
    /// <ul>
    ///   <li><c>Name</c> (string, required): Name of the attribute type</li>
    ///   <li><c>Description</c> (string, optional): Description of the attribute type</li>
    ///   <li><c>IsActive</c> (bool, optional): Whether the attribute type is active</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>int</c>: The ID of the newly created item attribute type</li>
    /// </ul>
    /// </remarks>
    [HttpPost("item-attribute-types")]
    public async Task<ActionResult<Result<int>>> Create(CreateItemAttributeTypeCommand command)
    {
        if (command == null)
        {
            return BadRequest(Result<int>.Validation("Command is required", []));
        }

        return await SendAsync(command);
    }

    /// <summary>
    /// Update an existing item attribute type.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Updates an existing item attribute type with the provided information.</li>
    ///   <li>Validates the input data and ensures the item attribute type exists.</li>
    /// </ul>
    /// <b>Request body:</b>
    /// <ul>
    ///   <li><c>Id</c> (int, required): ID of the attribute type to update</li>
    ///   <li><c>Name</c> (string, required): Updated name of the attribute type</li>
    ///   <li><c>Description</c> (string, optional): Updated description of the attribute type</li>
    ///   <li><c>IsActive</c> (bool, optional): Updated active status</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>bool</c>: Indicates success or failure</li>
    /// </ul>
    /// </remarks>
    [HttpPut("item-attribute-types/{id}")]
    public async Task<ActionResult<Result<bool>>> Update(int id, UpdateItemAttributeTypeCommand command)
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

    /// <summary>
    /// Delete an item attribute type.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Deletes an item attribute type by ID.</li>
    ///   <li>Returns NotFound if the item attribute type does not exist.</li>
    ///   <li>May fail if the attribute type is in use by other entities.</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>bool</c>: Indicates success or failure</li>
    /// </ul>
    /// </remarks>
    [HttpDelete("item-attribute-types/{id}")]
    public async Task<ActionResult<Result<bool>>> Delete(int id)
    {
        var command = new DeleteItemAttributeTypeCommand { Id = id };
        return await SendAsync(command);
    }
}
