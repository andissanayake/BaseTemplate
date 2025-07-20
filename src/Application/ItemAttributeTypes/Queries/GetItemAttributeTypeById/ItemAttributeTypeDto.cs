namespace BaseTemplate.Application.ItemAttributeTypes.Queries.GetItemAttributeTypeById;

public class ItemAttributeTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
} 