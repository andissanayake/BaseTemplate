using System.ComponentModel.DataAnnotations;

namespace BaseTemplate.Application.TenantFeatures.Items.Commands.GenerateVariant;

[Authorize(Roles = Roles.ItemManager)]
public record GenerateVariantsCommand : IRequest<bool>
{
    [Required]
    public int ItemId { get; init; }

    [Required]
    public List<int> CharacteristicTypeIds { get; init; } = [];
}
