﻿using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Common.RequestHandler;
using BaseTemplate.Application.Common.Security;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Events;

namespace BaseTemplate.Application.TodoItems.Commands.UpdateTodoItemStatus;

[Authorize]
public record UpdateTodoItemStatusCommand : IRequest<bool>
{
    public int Id { get; init; }
    public bool Done { get; init; }
}

public class UpdateTodoItemStatusCommandHandler : BaseRequestHandler<UpdateTodoItemStatusCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public UpdateTodoItemStatusCommandHandler(IUnitOfWorkFactory factory, IDomainEventDispatcher domainEventDispatcher, IIdentityService identityService) : base(identityService)
    {
        _factory = factory;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public override async Task<Result<bool>> HandleAsync(UpdateTodoItemStatusCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var entity = await uow.GetAsync<TodoItem>(request.Id);

        if (entity is null)
        {
            return Result<bool>.NotFound($"TodoItem with id {request.Id} not found.");
        }

        entity.Done = request.Done;
        await uow.UpdateAsync(entity);
        if (entity.Done) entity.AddDomainEvent(new TodoItemCompletedEvent(entity));
        uow.Commit();
        await _domainEventDispatcher.DispatchDomainEventsAsync(entity);
        return Result<bool>.Success(true);
    }
}
