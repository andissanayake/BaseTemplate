using BaseTemplate.Domain.Common;

namespace BaseTemplate.Application.Common.Events;

public interface IDomainEventHandler<TEvent> where TEvent : BaseEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
