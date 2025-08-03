using System.ComponentModel.DataAnnotations.Schema;
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

    [NotMapped]
    public string FullPath
    {
        get
        {
            var pathParts = new List<string>();
            var currentSpec = this;

            while (currentSpec != null)
            {
                pathParts.Insert(0, currentSpec.Name);
                currentSpec = currentSpec.ParentSpecification;
            }

            return string.Join(" / ", pathParts);
        }
    }
}
