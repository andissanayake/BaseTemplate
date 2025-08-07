namespace BaseTemplate.Application.TenantFeatures.Characteristics.Commands.DeleteCharacteristic;

[Authorize(Roles = Roles.CharacteristicManager)]
public record DeleteCharacteristicCommand(int Id) : IRequest<bool>;
