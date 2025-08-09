using System.ComponentModel.DataAnnotations;

namespace BaseTemplate.Application.TenantFeatures.Items.Commands.CreateItem;

[Authorize(Roles = Roles.ItemManager)]
public record CreateItemCommand : IRequest<int>
{
    [Required]
    [MaxLength(200, ErrorMessage = "The name cannot exceed 200 characters.")]
    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string? Tags { get; init; }

    [Required]
    public int SpecificationId { get; init; }

    public bool? HasVariant { get; init; }
}
