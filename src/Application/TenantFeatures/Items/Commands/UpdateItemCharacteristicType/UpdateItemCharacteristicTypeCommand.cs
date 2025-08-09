using System.ComponentModel.DataAnnotations;

namespace BaseTemplate.Application.TenantFeatures.Items.Commands.UpdateItemCharacteristicType;

[Authorize(Roles = Roles.ItemManager)]
public record UpdateItemCharacteristicTypeCommand : IRequest<bool>
{
    [Required]
    public int ItemId { get; init; }

    [Required]
    public List<int> CharacteristicTypeIds { get; init; } = [];
}
