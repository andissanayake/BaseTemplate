using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Specifications.Commands.CreateSpecification;
using BaseTemplate.Application.Specifications.Commands.DeleteSpecification;
using BaseTemplate.Application.Specifications.Commands.UpdateSpecification;
using BaseTemplate.Application.Specifications.Queries.GetSpecificationById;
using BaseTemplate.Application.Specifications.Queries.GetSpecifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[Authorize]
public class SpecificationsController : ApiControllerBase
{
    /// <summary>
    /// Get all root specifications for the current tenant with their children
    /// </summary>
    /// <returns>List of root specifications with hierarchical children</returns>
    [HttpGet]
    public async Task<ActionResult<Result<GetSpecificationsResponse>>> GetSpecifications()
    {
        return await SendAsync(new GetSpecificationsQuery());
    }

    /// <summary>
    /// Get specification by ID
    /// </summary>
    /// <param name="id">Specification ID</param>
    /// <returns>Specification details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Result<GetSpecificationByIdResponse>>> GetSpecification(int id)
    {
        return await SendAsync(new GetSpecificationByIdQuery(id));
    }

    /// <summary>
    /// Create a new specification
    /// </summary>
    /// <param name="command">Create specification command</param>
    /// <returns>Created specification ID</returns>
    [HttpPost]
    public async Task<ActionResult<Result<int>>> CreateSpecification(CreateSpecificationCommand command)
    {
        if (command == null)
        {
            return BadRequest(Result<int>.Validation("Command is required", []));
        }

        return await SendAsync(command);
    }

    /// <summary>
    /// Update an existing specification
    /// </summary>
    /// <param name="id">Specification ID</param>
    /// <param name="command">Update specification command</param>
    /// <returns>Success status</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<Result<bool>>> UpdateSpecification(int id, UpdateSpecificationCommand command)
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
    /// Delete a specification
    /// </summary>
    /// <param name="id">Specification ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<bool>>> DeleteSpecification(int id)
    {
        return await SendAsync(new DeleteSpecificationCommand(id));
    }
} 