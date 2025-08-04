using BaseTemplate.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

/// <summary>
/// Generic base controller for CRUD operations to reduce code duplication
/// </summary>
/// <typeparam name="TCreateCommand">Create command type</typeparam>
/// <typeparam name="TUpdateCommand">Update command type</typeparam>
/// <typeparam name="TDeleteCommand">Delete command type</typeparam>
/// <typeparam name="TGetByIdQuery">Get by ID query type</typeparam>
/// <typeparam name="TGetAllQuery">Get all query type</typeparam>
/// <typeparam name="TDto">Detail DTO type</typeparam>
/// <typeparam name="TBriefDto">Brief DTO type for lists</typeparam>
/// <typeparam name="TCreateResponse">Create response type (typically int for ID)</typeparam>
/// <typeparam name="TGetAllResponse">Get all response type (typically List&lt;TBriefDto&gt;)</typeparam>
public abstract class GenericCrudControllerBase<TCreateCommand, TUpdateCommand, TDeleteCommand, TGetByIdQuery, TGetAllQuery, TDto, TBriefDto, TCreateResponse, TGetAllResponse> 
    : ApiControllerBase
    where TCreateCommand : IRequest<TCreateResponse>
    where TUpdateCommand : IRequest<bool>
    where TDeleteCommand : IRequest<bool>
    where TGetByIdQuery : IRequest<TDto>
    where TGetAllQuery : IRequest<TGetAllResponse>
{
    /// <summary>
    /// Get all entities
    /// </summary>
    [HttpGet]
    public virtual async Task<ActionResult<Result<TGetAllResponse>>> GetAll()
    {
        var query = CreateGetAllQuery();
        return await SendAsync(query);
    }

    /// <summary>
    /// Get entity by ID
    /// </summary>
    [HttpGet("{id}")]
    public virtual async Task<ActionResult<Result<TDto>>> GetById(int id)
    {
        var query = CreateGetByIdQuery(id);
        return await SendAsync(query);
    }

    /// <summary>
    /// Create new entity
    /// </summary>
    [HttpPost]
    public virtual async Task<ActionResult<Result<TCreateResponse>>> Create([FromBody] TCreateCommand command)
    {
        return await SendAsync(command);
    }

    /// <summary>
    /// Update existing entity
    /// </summary>
    [HttpPut]
    public virtual async Task<ActionResult<Result<bool>>> Update([FromBody] TUpdateCommand command)
    {
        return await SendAsync(command);
    }

    /// <summary>
    /// Delete entity
    /// </summary>
    [HttpDelete("{id}")]
    public virtual async Task<ActionResult<Result<bool>>> Delete(int id)
    {
        var command = CreateDeleteCommand(id);
        return await SendAsync(command);
    }

    /// <summary>
    /// Create delete command with the specified ID
    /// </summary>
    protected abstract TDeleteCommand CreateDeleteCommand(int id);

    /// <summary>
    /// Create get by ID query with the specified ID
    /// </summary>
    protected abstract TGetByIdQuery CreateGetByIdQuery(int id);

    /// <summary>
    /// Create get all query
    /// </summary>
    protected abstract TGetAllQuery CreateGetAllQuery();
}