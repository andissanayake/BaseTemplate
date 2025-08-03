namespace BaseTemplate.Application.TenantFeatures.CharacteristicTypes.Queries.GetCharacteristicTypeById;

public class CharacteristicTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
