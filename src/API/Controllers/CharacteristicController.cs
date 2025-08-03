using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.TenantFeatures.Characteristics.Commands.CreateCharacteristic;
using BaseTemplate.Application.TenantFeatures.Characteristics.Commands.DeleteCharacteristic;
using BaseTemplate.Application.TenantFeatures.Characteristics.Commands.UpdateCharacteristic;
using BaseTemplate.Application.TenantFeatures.Characteristics.Queries.GetCharacteristicById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[Authorize]
[Route("api/characteristic")]
public class CharacteristicController : ApiControllerBase
{
    /// <summary>
    /// Create a new characteristic.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Creates a new characteristic for the current tenant</li>
///   <li>Validates that the characteristic type exists and belongs to the current tenant</li>
///   <li>Validates that the name and code are unique within the characteristic type</li>
    ///   <li>Automatically sets IsActive to true and associates with current tenant</li>
    ///   <li>Requires AttributeManager role permission</li>
    /// </ul>
    /// <b>Validation Rules:</b>
    /// <ul>
    ///   <li>Name is required and must be unique within the characteristic type</li>
///   <li>Code is required and must be unique within the characteristic type</li>
///   <li>Value is required</li>
///   <li>CharacteristicTypeId must reference an existing characteristic type</li>
    ///   <li>Maximum name length: 255 characters</li>
    ///   <li>Maximum code length: 50 characters</li>
    ///   <li>Maximum value length: 500 characters</li>
    /// </ul>
    /// <b>Request body:</b>
    /// <ul>
    ///   <li><c>Name</c> (string, required): Name of the characteristic (must be unique within characteristic type)</li>
///   <li><c>Code</c> (string, required): Code of the characteristic (must be unique within characteristic type)</li>
///   <li><c>Value</c> (string, required): Value of the characteristic</li>
///   <li><c>CharacteristicTypeId</c> (int, required): ID of the characteristic type this characteristic belongs to</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>int</c>: The ID of the newly created characteristic</li>
    /// </ul>
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateCharacteristicCommand command)
    {
        return await SendAsync(command);
    }

    /// <summary>
    /// Update an existing characteristic.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Updates an existing characteristic for the current tenant</li>
///   <li>Validates that the characteristic exists and belongs to the current tenant</li>
///   <li>Validates that the new name and code are unique within the characteristic type (if being changed)</li>
    ///   <li>Requires AttributeManager role permission</li>
    /// </ul>
    /// <b>Validation Rules:</b>
    /// <ul>
    ///   <li>Characteristic must exist and belong to current tenant</li>
///   <li>Name is required and must be unique within the characteristic type</li>
///   <li>Code is required and must be unique within the characteristic type</li>
    ///   <li>Value is required</li>
    ///   <li>Maximum name length: 255 characters</li>
    ///   <li>Maximum code length: 50 characters</li>
    ///   <li>Maximum value length: 500 characters</li>
    /// </ul>
    /// <b>Request body:</b>
    /// <ul>
    ///   <li><c>Id</c> (int, required): ID of the characteristic to update</li>
    ///   <li><c>Name</c> (string, required): Updated name of the characteristic (must be unique within characteristic type)</li>
///   <li><c>Code</c> (string, required): Updated code of the characteristic (must be unique within characteristic type)</li>
///   <li><c>Value</c> (string, required): Updated value of the characteristic</li>
///   <li><c>CharacteristicTypeId</c> (int, required): ID of the characteristic type this characteristic belongs to</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>bool</c>: Indicates success or failure</li>
    /// </ul>
    /// </remarks>
    [HttpPut]
    public async Task<ActionResult<Result<bool>>> Update([FromBody] UpdateCharacteristicCommand command)
    {
        return await SendAsync(command);
    }

    /// <summary>
    /// Soft delete a characteristic.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Performs a soft delete by setting IsActive to false</li>
///   <li>Validates that the characteristic exists and belongs to the current tenant</li>
///   <li>Does not physically delete the record (preserves data integrity)</li>
    ///   <li>Requires AttributeManager role permission</li>
    /// </ul>
    /// <b>Important Notes:</b>
    /// <ul>
    ///   <li>This is a soft delete - the record is not physically removed</li>
    ///   <li>Associated items will still reference this characteristic</li>
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
        return await SendAsync(new DeleteCharacteristicCommand(id));
    }

    /// <summary>
    /// Get a specific characteristic by ID.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Retrieves detailed information about a specific characteristic</li>
///   <li>Validates that the characteristic belongs to the current user's tenant</li>
///   <li>Returns NotFound if the characteristic does not exist or doesn't belong to the tenant</li>
    ///   <li>Requires AttributeManager role permission</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>CharacteristicDto</c>: Detailed characteristic information including Id, Name, Code, Value, IsActive, CharacteristicTypeId, CharacteristicTypeName</li>
    /// </ul>
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ActionResult<Result<CharacteristicDto>>> GetById(int id)
    {
        return await SendAsync(new GetICharacteristicByIdQuery(id));
    }
}
