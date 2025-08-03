namespace BaseTemplate.Application.TenantFeatures.Characteristics.Commands.CreateCharacteristic;

[Authorize(Roles = Roles.AttributeManager)]
public class CreateCharacteristicCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int CharacteristicTypeId { get; set; }
}
