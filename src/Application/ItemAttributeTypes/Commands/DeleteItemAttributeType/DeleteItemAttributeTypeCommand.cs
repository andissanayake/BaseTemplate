namespace BaseTemplate.Application.ItemAttributeTypes.Commands.DeleteItemAttributeType;
 
public class DeleteItemAttributeTypeCommand : IRequest<bool>
{
    public int Id { get; set; }
} 