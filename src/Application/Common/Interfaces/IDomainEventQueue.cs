using BaseTemplate.Domain.Common;

namespace BaseTemplate.Application.Common.Interfaces;
public interface IDomainEventQueue
{
    Task Enqueue(BaseEvent domainEvent);
}
