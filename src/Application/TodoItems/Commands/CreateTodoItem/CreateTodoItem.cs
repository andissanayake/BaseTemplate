﻿using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Events;

namespace BaseTemplate.Application.TodoItems.Commands.CreateTodoItem;

public record CreateTodoItemCommand : IRequest<int>
{
    public int ListId { get; init; }

    public string? Title { get; init; }
}

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, int>
{
    private readonly IUnitOfWorkFactory _factory;

    public CreateTodoItemCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<int> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoItem
        {
            ListId = request.ListId,
            Title = request.Title,
            Done = false
        };

        entity.AddDomainEvent(new TodoItemCreatedEvent(entity));
        var uow = _factory.CreateUOW();
        await uow.InsertAsync(entity);

        uow.Commit();

        return entity.Id;
    }
}
