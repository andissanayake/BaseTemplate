namespace BaseTemplate.Application.TenantFeatures.CharacteristicTypes.Queries.GetCharacteristicTypes;

[Authorize(Roles = Roles.AttributeManager)]
public class GetCharacteristicTypesQuery : IRequest<List<CharacteristicTypeBriefDto>>
{
}
