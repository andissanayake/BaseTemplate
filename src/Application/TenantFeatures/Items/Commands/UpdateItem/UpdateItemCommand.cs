using System.ComponentModel.DataAnnotations;

namespace BaseTemplate.Application.TenantFeatures.Items.Commands.UpdateItem;

[Authorize(Roles = Roles.ItemManager)]
public record UpdateItemCommand : IRequest<bool>
{
    [Required]
    public int Id { get; init; }

    [Required]
    [MaxLength(200, ErrorMessage = "The name cannot exceed 200 characters.")]
    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string? Tags { get; init; }
    public bool IsActive { get; init; }

    [Required]
    public int SpecificationId { get; init; }
}
