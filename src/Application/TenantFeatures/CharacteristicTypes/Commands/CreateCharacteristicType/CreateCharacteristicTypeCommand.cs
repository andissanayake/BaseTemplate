namespace BaseTemplate.Application.TenantFeatures.CharacteristicTypes.Commands.CreateCharacteristicType;

[Authorize(Roles = Roles.CharacteristicManager)]
public class CreateCharacteristicTypeCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
