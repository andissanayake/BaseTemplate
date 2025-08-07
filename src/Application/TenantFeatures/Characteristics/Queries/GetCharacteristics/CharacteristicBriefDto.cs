namespace BaseTemplate.Application.TenantFeatures.Characteristics.Queries.GetCharacteristics;

public class CharacteristicBriefDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int CharacteristicTypeId { get; set; }
    public string CharacteristicTypeName { get; set; } = string.Empty;
}
