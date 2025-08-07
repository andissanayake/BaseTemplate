namespace BaseTemplate.Application.TenantFeatures.Characteristics.Queries.GetCharacteristics;

[Authorize(Roles = Roles.CharacteristicManager)]
public class GetCharacteristicsQuery : IRequest<List<CharacteristicBriefDto>>
{
    public int CharacteristicTypeId { get; set; }
}
