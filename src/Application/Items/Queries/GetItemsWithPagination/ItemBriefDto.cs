namespace BaseTemplate.Application.Items.Queries.GetItemsWithPagination;

public class ItemBriefDto
{
    public int Id { get; init; }
    public int TenantId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public bool IsActive { get; init; }
    public string? Category { get; init; }
} 