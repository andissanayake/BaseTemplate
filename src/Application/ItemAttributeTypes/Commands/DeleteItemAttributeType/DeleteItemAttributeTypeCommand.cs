namespace BaseTemplate.Application.ItemAttributeTypes.Commands.DeleteItemAttributeType;

[Authorize(Roles = Roles.AttributeManager)]
public class DeleteItemAttributeTypeCommand : IRequest<bool>
{
    public int Id { get; set; }
}
