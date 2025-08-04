using Microsoft.EntityFrameworkCore;
using BaseTemplate.Domain.Common;

namespace BaseTemplate.Application.Common.Handlers;

/// <summary>
/// Generic base class for create command handlers
/// </summary>
public abstract class CreateCommandHandlerBase<TCommand, TEntity> : IRequestHandler<TCommand, int>
    where TCommand : IRequest<int>
    where TEntity : BaseEntity, new()
{
    protected readonly IAppDbContext Context;
    
    protected CreateCommandHandlerBase(IAppDbContext context)
    {
        Context = context;
    }
    
    public async Task<Result<int>> HandleAsync(TCommand request, CancellationToken cancellationToken)
    {
        // Perform validation
        var validationResult = await ValidateAsync(request, cancellationToken);
        if (!ResultCodeMapper.IsSuccess(validationResult.Code))
        {
            return Result<int>.Validation(validationResult.Message!, validationResult.Details);
        }
        
        // Map command to entity
        var entity = await MapToEntityAsync(request, cancellationToken);
        
        // Add entity to context
        var dbSet = Context.Set<TEntity>();
        dbSet.Add(entity);
        
        // Save changes
        await Context.SaveChangesAsync(cancellationToken);
        
        return Result<int>.Success(entity.Id);
    }
    
    /// <summary>
    /// Validate the command before processing
    /// </summary>
    protected abstract Task<Result> ValidateAsync(TCommand request, CancellationToken cancellationToken);
    
    /// <summary>
    /// Map the command to an entity
    /// </summary>
    protected abstract Task<TEntity> MapToEntityAsync(TCommand request, CancellationToken cancellationToken);
}

/// <summary>
/// Generic base class for update command handlers
/// </summary>
public abstract class UpdateCommandHandlerBase<TCommand, TEntity> : IRequestHandler<TCommand, bool>
    where TCommand : IRequest<bool>
    where TEntity : BaseEntity
{
    protected readonly IAppDbContext Context;
    
    protected UpdateCommandHandlerBase(IAppDbContext context)
    {
        Context = context;
    }
    
    public async Task<Result<bool>> HandleAsync(TCommand request, CancellationToken cancellationToken)
    {
        // Get entity ID from command
        var entityId = GetEntityId(request);
        
        // Find existing entity
        var entity = await GetEntityAsync(entityId, cancellationToken);
        if (entity == null)
        {
            return Result<bool>.NotFound($"{typeof(TEntity).Name} not found");
        }
        
        // Perform validation
        var validationResult = await ValidateAsync(request, entity, cancellationToken);
        if (!ResultCodeMapper.IsSuccess(validationResult.Code))
        {
            return Result<bool>.Validation(validationResult.Message!, validationResult.Details);
        }
        
        // Update entity
        await UpdateEntityAsync(request, entity, cancellationToken);
        
        // Save changes
        await Context.SaveChangesAsync(cancellationToken);
        
        return Result<bool>.Success(true);
    }
    
    /// <summary>
    /// Get the entity ID from the command
    /// </summary>
    protected abstract int GetEntityId(TCommand request);
    
    /// <summary>
    /// Get the entity by ID
    /// </summary>
    protected virtual async Task<TEntity?> GetEntityAsync(int id, CancellationToken cancellationToken)
    {
        return await Context.Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    
    /// <summary>
    /// Validate the command before processing
    /// </summary>
    protected abstract Task<Result> ValidateAsync(TCommand request, TEntity entity, CancellationToken cancellationToken);
    
    /// <summary>
    /// Update the entity with data from the command
    /// </summary>
    protected abstract Task UpdateEntityAsync(TCommand request, TEntity entity, CancellationToken cancellationToken);
}

/// <summary>
/// Generic base class for delete command handlers
/// </summary>
public abstract class DeleteCommandHandlerBase<TCommand, TEntity> : IRequestHandler<TCommand, bool>
    where TCommand : IRequest<bool>
    where TEntity : BaseEntity
{
    protected readonly IAppDbContext Context;
    
    protected DeleteCommandHandlerBase(IAppDbContext context)
    {
        Context = context;
    }
    
    public async Task<Result<bool>> HandleAsync(TCommand request, CancellationToken cancellationToken)
    {
        // Get entity ID from command
        var entityId = GetEntityId(request);
        
        // Find existing entity
        var entity = await GetEntityAsync(entityId, cancellationToken);
        if (entity == null)
        {
            return Result<bool>.NotFound($"{typeof(TEntity).Name} not found");
        }
        
        // Perform validation
        var validationResult = await ValidateAsync(request, entity, cancellationToken);
        if (!ResultCodeMapper.IsSuccess(validationResult.Code))
        {
            return Result<bool>.Validation(validationResult.Message!, validationResult.Details);
        }
        
        // Perform deletion (soft delete by setting IsActive = false)
        await DeleteEntityAsync(entity, cancellationToken);
        
        // Save changes
        await Context.SaveChangesAsync(cancellationToken);
        
        return Result<bool>.Success(true);
    }
    
    /// <summary>
    /// Get the entity ID from the command
    /// </summary>
    protected abstract int GetEntityId(TCommand request);
    
    /// <summary>
    /// Get the entity by ID
    /// </summary>
    protected virtual async Task<TEntity?> GetEntityAsync(int id, CancellationToken cancellationToken)
    {
        return await Context.Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    
    /// <summary>
    /// Validate the command before processing
    /// </summary>
    protected virtual Task<Result> ValidateAsync(TCommand request, TEntity entity, CancellationToken cancellationToken)
    {
        // Default implementation - no additional validation
        return Task.FromResult(Result.Success());
    }
    
    /// <summary>
    /// Delete the entity (soft delete by default)
    /// </summary>
    protected virtual Task DeleteEntityAsync(TEntity entity, CancellationToken cancellationToken)
    {
        // Check if entity supports soft delete
        if (entity is BaseAuditableEntity auditableEntity)
        {
            auditableEntity.IsActive = false;
        }
        else
        {
            // Hard delete if no soft delete support
            Context.Set<TEntity>().Remove(entity);
        }
        
        return Task.CompletedTask;
    }
}

/// <summary>
/// Generic base class for query handlers that retrieve entities by ID
/// </summary>
public abstract class GetByIdQueryHandlerBase<TQuery, TEntity, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IRequest<TResponse>
    where TEntity : BaseEntity
{
    protected readonly IAppDbContext Context;
    
    protected GetByIdQueryHandlerBase(IAppDbContext context)
    {
        Context = context;
    }
    
    public async Task<Result<TResponse>> HandleAsync(TQuery request, CancellationToken cancellationToken)
    {
        var entityId = GetEntityId(request);
        
        var query = BuildQuery(entityId);
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        
        if (entity == null)
        {
            return Result<TResponse>.NotFound($"{typeof(TEntity).Name} not found");
        }
        
        var response = await MapToResponseAsync(entity, cancellationToken);
        return Result<TResponse>.Success(response);
    }
    
    /// <summary>
    /// Get the entity ID from the query
    /// </summary>
    protected abstract int GetEntityId(TQuery request);
    
    /// <summary>
    /// Build the query for retrieving the entity
    /// </summary>
    protected virtual IQueryable<TEntity> BuildQuery(int id)
    {
        return Context.Set<TEntity>().Where(e => e.Id == id);
    }
    
    /// <summary>
    /// Map the entity to the response DTO
    /// </summary>
    protected abstract Task<TResponse> MapToResponseAsync(TEntity entity, CancellationToken cancellationToken);
}