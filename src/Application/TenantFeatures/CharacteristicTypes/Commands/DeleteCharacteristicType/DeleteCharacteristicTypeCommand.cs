namespace BaseTemplate.Application.TenantFeatures.CharacteristicTypes.Commands.DeleteCharacteristicType;

[Authorize(Roles = Roles.CharacteristicManager)]
public class DeleteCharacteristicTypeCommand : IRequest<bool>
{
    public int Id { get; set; }
}
