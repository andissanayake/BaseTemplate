namespace BaseTemplate.Application.TenantFeatures.Items.Queries.GetItemVariantByItemId;

public class ItemVariantDto
{
    public int Id { get; init; }
    public int ItemId { get; init; }
    public string Code { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public List<ItemVariantCharacteristicDto> Characteristics { get; init; } = [];
    public ItemDetailDto Item { get; init; } = new();
}

public class ItemDetailDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public string? Tags { get; init; }
    public int SpecificationId { get; init; }
    public string SpecificationFullPath { get; init; } = string.Empty;
    public bool HasVariant { get; init; }
}

public class ItemVariantCharacteristicDto
{
    public int Id { get; init; }
    public int CharacteristicId { get; init; }
    public string CharacteristicName { get; init; } = string.Empty;
    public string CharacteristicCode { get; init; } = string.Empty;
    public string? CharacteristicValue { get; init; }
    public int CharacteristicTypeId { get; init; }
    public string CharacteristicTypeName { get; init; } = string.Empty;
}