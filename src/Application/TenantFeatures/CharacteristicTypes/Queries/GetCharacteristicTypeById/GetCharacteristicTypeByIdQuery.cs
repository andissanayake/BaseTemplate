namespace BaseTemplate.Application.TenantFeatures.CharacteristicTypes.Queries.GetCharacteristicTypeById;

[Authorize(Roles = Roles.CharacteristicManager)]
public class GetCharacteristicTypeByIdQuery : IRequest<CharacteristicTypeDto>
{
    public int Id { get; set; }
}
