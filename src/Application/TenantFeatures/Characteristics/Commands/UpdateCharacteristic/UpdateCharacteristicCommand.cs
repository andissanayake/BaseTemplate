namespace BaseTemplate.Application.TenantFeatures.Characteristics.Commands.UpdateCharacteristic;

[Authorize(Roles = Roles.AttributeManager)]
public class UpdateCharacteristicCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
