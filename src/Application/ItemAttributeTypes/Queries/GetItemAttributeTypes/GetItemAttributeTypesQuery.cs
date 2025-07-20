namespace BaseTemplate.Application.ItemAttributeTypes.Queries.GetItemAttributeTypes;
 
public class GetItemAttributeTypesQuery : IRequest<List<ItemAttributeTypeBriefDto>>
{
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
} 