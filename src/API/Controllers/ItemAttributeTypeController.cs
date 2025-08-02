using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.TenantFeatures.ItemAttributes.Queries.GetItemAttributes;
using BaseTemplate.Application.TenantFeatures.ItemAttributeTypes.Commands.CreateItemAttributeType;
using BaseTemplate.Application.TenantFeatures.ItemAttributeTypes.Commands.DeleteItemAttributeType;
using BaseTemplate.Application.TenantFeatures.ItemAttributeTypes.Commands.UpdateItemAttributeType;
using BaseTemplate.Application.TenantFeatures.ItemAttributeTypes.Queries.GetItemAttributeTypeById;
using BaseTemplate.Application.TenantFeatures.ItemAttributeTypes.Queries.GetItemAttributeTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[Authorize]
[Route("api/item-attribute-type")]
public class ItemAttributeTypeController : ApiControllerBase
{
    /// <summary>
    /// Get all item attribute types for the current tenant.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Retrieves all item attribute types belonging to the current user's tenant</li>
    ///   <li>Returns only active attribute types (where IsActive = true)</li>
    ///   <li>Results are ordered by creation date (newest first)</li>
    ///   <li>Requires AttributeManager role permission</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>List&lt;ItemAttributeTypeBriefDto&gt;</c>: List of item attribute types with basic information including Id, Name, Description, IsActive</li>
    /// </ul>
    /// </remarks>
    [HttpGet]
    public async Task<ActionResult<Result<List<ItemAttributeTypeBriefDto>>>> GetAll()
    {
        var query = new GetItemAttributeTypesQuery();

        return await SendAsync(query);
    }

    /// <summary>
    /// Get a specific item attribute type by ID.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Retrieves detailed information about a specific item attribute type</li>
    ///   <li>Validates that the attribute type belongs to the current user's tenant</li>
    ///   <li>Returns NotFound if the item attribute type does not exist or doesn't belong to the tenant</li>
    ///   <li>Requires AttributeManager role permission</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>ItemAttributeTypeDto</c>: Detailed item attribute type information including Id, Name, Description, IsActive</li>
    /// </ul>
    /// </remarks>
    [HttpGet("{id}")]
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
    ///   <li>Creates a new item attribute type for the current tenant</li>
    ///   <li>Validates that the name is unique within the tenant (case-sensitive)</li>
    ///   <li>Automatically sets IsActive to true and associates with current tenant</li>
    ///   <li>Requires AttributeManager role permission</li>
    /// </ul>
    /// <b>Validation Rules:</b>
    /// <ul>
    ///   <li>Name is required and must be unique within the tenant</li>
    ///   <li>Description is optional</li>
    ///   <li>Maximum name length: 255 characters</li>
    /// </ul>
    /// <b>Request body:</b>
    /// <ul>
    ///   <li><c>Name</c> (string, required): Name of the attribute type (must be unique within tenant)</li>
    ///   <li><c>Description</c> (string, optional): Description of the attribute type</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>int</c>: The ID of the newly created item attribute type</li>
    /// </ul>
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create(CreateItemAttributeTypeCommand command)
    {
        return await SendAsync(command);
    }

    /// <summary>
    /// Update an existing item attribute type.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Updates an existing item attribute type for the current tenant</li>
    ///   <li>Validates that the attribute type exists and belongs to the current tenant</li>
    ///   <li>Validates that the new name is unique within the tenant (if name is being changed)</li>
    ///   <li>Requires AttributeManager role permission</li>
    /// </ul>
    /// <b>Validation Rules:</b>
    /// <ul>
    ///   <li>Attribute type must exist and belong to current tenant</li>
    ///   <li>Name is required and must be unique within the tenant</li>
    ///   <li>Description is optional</li>
    ///   <li>Maximum name length: 255 characters</li>
    /// </ul>
    /// <b>Request body:</b>
    /// <ul>
    ///   <li><c>Id</c> (int, required): ID of the attribute type to update</li>
    ///   <li><c>Name</c> (string, required): Updated name of the attribute type (must be unique within tenant)</li>
    ///   <li><c>Description</c> (string, optional): Updated description of the attribute type</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>bool</c>: Indicates success or failure</li>
    /// </ul>
    /// </remarks>
    [HttpPut]
    public async Task<ActionResult<Result<bool>>> Update(UpdateItemAttributeTypeCommand command)
    {
        return await SendAsync(command);
    }

    /// <summary>
    /// Soft delete an item attribute type.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Performs a soft delete by setting IsActive to false</li>
    ///   <li>Validates that the attribute type exists and belongs to the current tenant</li>
    ///   <li>Does not physically delete the record (preserves data integrity)</li>
    ///   <li>Requires AttributeManager role permission</li>
    /// </ul>
    /// <b>Important Notes:</b>
    /// <ul>
    ///   <li>This is a soft delete - the record is not physically removed</li>
    ///   <li>Associated item attributes will still reference this type</li>
    ///   <li>Consider the impact on existing item attributes before deletion</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>bool</c>: Indicates success or failure</li>
    /// </ul>
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<bool>>> Delete(int id)
    {
        var command = new DeleteItemAttributeTypeCommand { Id = id };
        return await SendAsync(command);
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
    [HttpGet("{id}/item-attribute")]
    public async Task<ActionResult<Result<List<ItemAttributeBriefDto>>>> GetAll(
        [FromRoute] int id)
    {
        var query = new GetItemAttributesQuery
        {
            ItemAttributeTypeId = id
        };

        return await SendAsync(query);
    }
}
