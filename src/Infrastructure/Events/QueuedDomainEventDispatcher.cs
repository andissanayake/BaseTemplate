using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Common;

namespace BaseTemplate.Infrastructure.Events;

public class QueuedDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IDomainEventQueue _queue;

    public QueuedDomainEventDispatcher(IDomainEventQueue queue)
    {
        _queue = queue;
    }

    public Task DispatchDomainEventsAsync(params BaseEntity[] entities)
    {
        foreach (var entity in entities)
        {
            var events = entity.DomainEvents.ToList();
            entity.ClearDomainEvents();

            foreach (var domainEvent in events)
            {
                _queue.Enqueue(domainEvent);
            }
        }

        return Task.CompletedTask;
    }
}
