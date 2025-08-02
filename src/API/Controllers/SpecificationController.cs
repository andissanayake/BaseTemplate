using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.TenantFeatures.Specifications.Commands.CreateSpecification;
using BaseTemplate.Application.TenantFeatures.Specifications.Commands.DeleteSpecification;
using BaseTemplate.Application.TenantFeatures.Specifications.Commands.UpdateSpecification;
using BaseTemplate.Application.TenantFeatures.Specifications.Queries.GetSpecificationById;
using BaseTemplate.Application.TenantFeatures.Specifications.Queries.GetSpecifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[Authorize]
[Route("api/specification")]
public class SpecificationController : ApiControllerBase
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
        return await SendAsync(command);
    }

    /// <summary>
    /// Update an existing specification
    /// </summary>
    /// <param name="command">Update specification command</param>
    /// <returns>Success status</returns>
    [HttpPut]
    public async Task<ActionResult<Result<bool>>> UpdateSpecification(UpdateSpecificationCommand command)
    {
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
