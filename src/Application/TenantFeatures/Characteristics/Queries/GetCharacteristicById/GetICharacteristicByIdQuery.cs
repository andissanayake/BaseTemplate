namespace BaseTemplate.Application.TenantFeatures.Characteristics.Queries.GetCharacteristicById;

[Authorize(Roles = Roles.AttributeManager)]
public record GetICharacteristicByIdQuery(int Id) : IRequest<CharacteristicDto>;
