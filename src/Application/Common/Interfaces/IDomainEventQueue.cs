using BaseTemplate.Domain.Common;

namespace BaseTemplate.Application.Common.Interfaces;
public interface IDomainEventQueue
{
    void Enqueue(BaseEvent domainEvent);
}
