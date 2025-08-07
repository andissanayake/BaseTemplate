namespace BaseTemplate.Application.TenantFeatures.CharacteristicTypes.Queries.GetCharacteristicTypes;

[Authorize(Roles = Roles.CharacteristicManager)]
public class GetCharacteristicTypesQuery : IRequest<List<CharacteristicTypeBriefDto>>
{
}
