

namespace BaseTemplate.Domain.Common;

public abstract class BaseEvent
{
    public Guid EventId { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
}
