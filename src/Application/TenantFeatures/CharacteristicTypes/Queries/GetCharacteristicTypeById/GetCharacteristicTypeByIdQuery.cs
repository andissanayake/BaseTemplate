namespace BaseTemplate.Application.TenantFeatures.CharacteristicTypes.Queries.GetCharacteristicTypeById;

[Authorize(Roles = Roles.AttributeManager)]
public class GetCharacteristicTypeByIdQuery : IRequest<CharacteristicTypeDto>
{
    public int Id { get; set; }
}
