namespace BaseTemplate.Application.TenantFeatures.Items.Queries.GetItemsWithPagination;

public class ItemBriefDto
{
    public int Id { get; init; }
    public int TenantId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public string? Tags { get; init; }
    public int SpecificationId { get; init; }
    public string SpecificationFullPath { get; init; } = string.Empty;
    public bool? HasVariant { get; init; }
}
