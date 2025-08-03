namespace BaseTemplate.Application.TenantFeatures.Items.Queries.GetItemById;

public class ItemDto
{
    public int Id { get; init; }
    public int TenantId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public bool IsActive { get; init; }
    public string? Category { get; init; }
    public int SpecificationId { get; init; }
    public string SpecificationFullPath { get; init; } = string.Empty;
} 