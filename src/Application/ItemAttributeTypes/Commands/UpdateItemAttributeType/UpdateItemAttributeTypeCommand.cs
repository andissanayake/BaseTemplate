namespace BaseTemplate.Application.ItemAttributeTypes.Commands.UpdateItemAttributeType;

public class UpdateItemAttributeTypeCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
} 