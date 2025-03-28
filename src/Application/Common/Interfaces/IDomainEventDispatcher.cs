using BaseTemplate.Domain.Common;

namespace BaseTemplate.Application.Common.Interfaces;
public interface IDomainEventDispatcher
{
    Task DispatchDomainEventsAsync(params BaseEntity[] entities);
}
