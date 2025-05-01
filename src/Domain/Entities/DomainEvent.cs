namespace BaseTemplate.Domain.Entities;
public class DomainEvent : BaseAuditableEntity
{
    public Guid EventId { get; set; }
    public required string EventType { get; set; }
    public required string EventData { get; set; }
    public required string Status { get; set; }      // Status of the event ("Pending", "Processed", "Failed")
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public string? Result { get; set; }
}
