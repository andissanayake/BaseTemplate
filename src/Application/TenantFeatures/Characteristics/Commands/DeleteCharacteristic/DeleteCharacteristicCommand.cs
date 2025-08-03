namespace BaseTemplate.Application.TenantFeatures.Characteristics.Commands.DeleteCharacteristic;

[Authorize(Roles = Roles.AttributeManager)]
public record DeleteCharacteristicCommand(int Id) : IRequest<bool>;
