namespace BaseTemplate.Application.TenantFeatures.Characteristics.Queries.GetCharacteristicById;

public class CharacteristicDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int ItemAttributeTypeId { get; set; }
    public string ItemAttributeTypeName { get; set; } = string.Empty;
} 
