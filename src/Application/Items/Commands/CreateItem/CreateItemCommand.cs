using System.ComponentModel.DataAnnotations;

namespace BaseTemplate.Application.Items.Commands.CreateItem;

[Authorize(Roles = Roles.ItemManager)]
public record CreateItemCommand() : IRequest<int>
{
    [Required]
    [MaxLength(200, ErrorMessage = "The name cannot exceed 200 characters.")]
    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.")]
    public decimal Price { get; init; }
    public string? Category { get; init; }
}
