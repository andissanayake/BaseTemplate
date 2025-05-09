﻿using BaseTemplate.Application.Common.Events;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Events;
using Microsoft.Extensions.Logging;

namespace BaseTemplate.Application.TodoItems.EventHandlers;

public class TodoItemCreatedEventHandler : IDomainEventHandler<TodoItemCreatedEvent>
{
    private readonly ILogger<TodoItemCreatedEventHandler> _logger;
    private readonly IUnitOfWorkFactory _factory;
    public TodoItemCreatedEventHandler(ILogger<TodoItemCreatedEventHandler> logger, IUnitOfWorkFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    public async Task HandleAsync(TodoItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var count = await uow.QueryFirstOrDefaultAsync<int>("Select count(1) from TodoItem");
        _logger.LogInformation("Sample Domain Event: {DomainEvent} - {count}", notification.GetType().Name, count);
    }
}
