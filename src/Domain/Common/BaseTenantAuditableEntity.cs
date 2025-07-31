using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Domain.Common;

public abstract class BaseTenantAuditableEntity : BaseAuditableEntity
{
    public int TenantId { get; set; }
    public Tenant? Tenant { get; set; }
}
