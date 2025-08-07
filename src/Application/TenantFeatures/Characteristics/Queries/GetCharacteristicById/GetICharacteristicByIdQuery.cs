namespace BaseTemplate.Application.TenantFeatures.Characteristics.Queries.GetCharacteristicById;

[Authorize(Roles = Roles.CharacteristicManager)]
public record GetICharacteristicByIdQuery(int Id) : IRequest<CharacteristicDto>;
