using System.Text.Json.Serialization;

namespace BaseTemplate.Domain.Entities;
public class Specification : BaseTenantAuditableEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int? ParentSpecificationId { get; set; }
    public Specification? ParentSpecification { get; set; }

    [JsonIgnore]
    public List<Specification>? Children { get; set; }

}
