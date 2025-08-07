namespace BaseTemplate.Application.TenantFeatures.Items.Queries.GetItemById;

public class ItemDto
{
    public int Id { get; init; }
    public int TenantId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public string? Tags { get; init; }
    public int SpecificationId { get; init; }
    public string SpecificationFullPath { get; init; } = string.Empty;
    public List<ItemCharacteristicTypeDto> CharacteristicTypes { get; init; } = [];
}

public class ItemCharacteristicTypeDto
{
    public int Id { get; init; }
    public int CharacteristicTypeId { get; init; }
    public string CharacteristicTypeName { get; init; } = string.Empty;
    public string? CharacteristicTypeDescription { get; init; }
} 