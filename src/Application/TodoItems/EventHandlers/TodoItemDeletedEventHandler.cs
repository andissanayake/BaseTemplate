using BaseTemplate.Application.Common.Events;
using BaseTemplate.Domain.Events;
using Microsoft.Extensions.Logging;

namespace BaseTemplate.Application.TodoItems.EventHandlers;

public class TodoItemDeletedEventHandler : IDomainEventHandler<TodoItemDeletedEvent>
{
    private readonly ILogger<TodoItemCompletedEventHandler> _logger;

    public TodoItemDeletedEventHandler(ILogger<TodoItemCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(TodoItemDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sample Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
