using BaseTemplate.Application.Common.Events;
using BaseTemplate.Domain.Events;
using Microsoft.Extensions.Logging;

namespace BaseTemplate.Application.TodoItems.EventHandlers;

public class TodoItemCompletedEventHandler : IDomainEventHandler<TodoItemCompletedEvent>
{
    private readonly ILogger<TodoItemCompletedEventHandler> _logger;

    public TodoItemCompletedEventHandler(ILogger<TodoItemCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(TodoItemCompletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sample Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
