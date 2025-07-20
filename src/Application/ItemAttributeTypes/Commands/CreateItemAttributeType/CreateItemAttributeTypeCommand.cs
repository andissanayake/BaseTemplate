namespace BaseTemplate.Application.ItemAttributeTypes.Commands.CreateItemAttributeType;
 
public class CreateItemAttributeTypeCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
} 