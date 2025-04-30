

namespace BaseTemplate.Domain.Common;

public abstract class BaseEvent
{
    public Guid EventId { get; private set; }
    public DateTimeOffset CreatedDate { get; private set; }

    protected BaseEvent()
    {
        EventId = Guid.NewGuid();
        CreatedDate = DateTimeOffset.Now;
    }
}
