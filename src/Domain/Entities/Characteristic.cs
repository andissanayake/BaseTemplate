namespace BaseTemplate.Domain.Entities;

public class Characteristic : BaseTenantAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int CharacteristicTypeId { get; set; }
    public CharacteristicType CharacteristicType { get; set; } = default!;

}
