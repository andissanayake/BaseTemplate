namespace BaseTemplate.Application.ItemAttributeTypes.Queries.GetItemAttributeTypeById;
 
public class GetItemAttributeTypeByIdQuery : IRequest<ItemAttributeTypeDto>
{
    public int Id { get; set; }
} 