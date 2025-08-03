namespace BaseTemplate.Application.TenantFeatures.CharacteristicTypes.Commands.DeleteCharacteristicType;

[Authorize(Roles = Roles.AttributeManager)]
public class DeleteCharacteristicTypeCommand : IRequest<bool>
{
    public int Id { get; set; }
}
