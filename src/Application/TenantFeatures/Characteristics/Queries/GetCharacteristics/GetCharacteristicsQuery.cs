namespace BaseTemplate.Application.TenantFeatures.Characteristics.Queries.GetCharacteristics;

[Authorize(Roles = Roles.AttributeManager)]
public class GetCharacteristicsQuery : IRequest<List<CharacteristicBriefDto>>
{
    public int ItemAttributeTypeId { get; set; }
}
