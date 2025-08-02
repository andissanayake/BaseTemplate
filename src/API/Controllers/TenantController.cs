using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.GlobalFeatures.Tenants.Commands.CreateTenant;
using BaseTemplate.Application.TenantFeatures.Tenants.Commands.UpdateTenant;
using BaseTemplate.Application.TenantFeatures.Tenants.Queries.GetTenantById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[Authorize]
[Route("api/tenant")]
public class TenantController : ApiControllerBase
{
    /// <summary>
    /// Get the current user's tenant details.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Fetches the tenant associated with the current authenticated user.</li>
    ///   <li>If the user does not have a tenant, returns NotFound.</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>Id</c> (int): Tenant ID</li>
    ///   <li><c>Name</c> (string): Tenant name</li>
    ///   <li><c>Address</c> (string, optional): Tenant address</li>
    /// </ul>
    /// </remarks>
    [HttpGet]
    public async Task<ActionResult<Result<GetTenantResponse>>> GetById()
    {
        return await SendAsync(new GetTenantQuery());
    }

    /// <summary>
    /// Create a new tenant and assign the current user as the owner.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Checks if the user already has a tenant. If so, returns the existing tenant ID and does not create a new tenant.</li>
    ///   <li>If not, creates a new tenant with the provided name and address, and sets the current user as the owner.</li>
    ///   <li>Updates the user's tenant association in the database.</li>
    ///   <li>Adds the <c>TenantOwner</c> role to the user.</li>
    ///   <li>Invalidates the user profile cache to ensure fresh data on subsequent requests.</li>
    ///   <li>Any authenticated user can perform this action.</li>
    /// </ul>
    /// <b>Request body:</b>
    /// <ul>
    ///   <li><c>Name</c> (string, required): Tenant name (2-100 characters)</li>
    ///   <li><c>Address</c> (string, optional): Tenant address (max 500 characters)</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>int</c>: The new or existing tenant ID</li>
    /// </ul>
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create(CreateTenantCommand command)
    {
        return await SendAsync(command);
    }

    /// <summary>
    /// Update the details of the current user's tenant.
    /// </summary>
    /// <remarks>
    /// <b>What this endpoint does:</b>
    /// <ul>
    ///   <li>Updates the name and address of the tenant associated with the current user.</li>
    ///   <li>Only users with the <c>TenantManager</c> role can perform this action.</li>
    ///   <li>Fails with NotFound if the tenant does not exist for the user.</li>
    /// </ul>
    /// <b>Request body:</b>
    /// <ul>
    ///   <li><c>Name</c> (string, required): New tenant name (2-100 characters)</li>
    ///   <li><c>Address</c> (string, optional): New tenant address (max 500 characters)</li>
    /// </ul>
    /// <b>Response:</b>
    /// <ul>
    ///   <li><c>bool</c>: Indicates success or failure</li>
    /// </ul>
    /// </remarks>
    [HttpPut]
    public async Task<ActionResult<Result<bool>>> Update(UpdateTenantCommand command)
    {
        return await SendAsync(command);
    }
}
