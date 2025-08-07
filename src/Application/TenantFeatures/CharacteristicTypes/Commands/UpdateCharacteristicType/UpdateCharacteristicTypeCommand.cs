namespace BaseTemplate.Application.TenantFeatures.CharacteristicTypes.Commands.UpdateCharacteristicType;

[Authorize(Roles = Roles.CharacteristicManager)]
public class UpdateCharacteristicTypeCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
