namespace BaseTemplate.Application.ItemAttributeTypes.Queries.GetItemAttributeTypes;

public class ItemAttributeTypeBriefDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset Created { get; set; }
    public string? CreatedBy { get; set; }
} 