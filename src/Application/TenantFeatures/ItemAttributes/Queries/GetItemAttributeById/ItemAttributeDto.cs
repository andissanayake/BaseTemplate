namespace BaseTemplate.Application.TenantFeatures.ItemAttributes.Queries.GetItemAttributeById;

public class ItemAttributeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int ItemAttributeTypeId { get; set; }
    public string ItemAttributeTypeName { get; set; } = string.Empty;
} 