﻿using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Enums;

namespace BaseTemplate.Application.TodoItems.Commands.UpdateTodoItemDetail;

public record UpdateTodoItemDetailCommand : IRequest
{
    public int Id { get; init; }

    public int ListId { get; init; }

    public PriorityLevel Priority { get; init; }

    public string? Note { get; init; }
}

public class UpdateTodoItemDetailCommandHandler : IRequestHandler<UpdateTodoItemDetailCommand>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public UpdateTodoItemDetailCommandHandler(IUnitOfWorkFactory factory, IDomainEventDispatcher domainEventDispatcher)
    {
        _factory = factory;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task Handle(UpdateTodoItemDetailCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var entity = await uow.GetAsync<TodoItem>(request.Id);

        Guard.Against.NotFound(request.Id, entity);

        entity.ListId = request.ListId;
        entity.Priority = request.Priority;
        entity.Note = request.Note;
        await uow.UpdateAsync(entity);
        uow.Commit();
        await _domainEventDispatcher.DispatchDomainEventsAsync(entity);
    }
}
