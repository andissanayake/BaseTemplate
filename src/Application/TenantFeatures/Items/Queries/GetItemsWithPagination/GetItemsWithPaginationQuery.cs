using System.ComponentModel.DataAnnotations;

namespace BaseTemplate.Application.TenantFeatures.Items.Queries.GetItemsWithPagination;

[Authorize(Roles = Roles.ItemManager)]
public record GetItemsWithPaginationQuery : IRequest<PaginatedList<ItemBriefDto>>
{
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than or equal to 1.")]
    public int PageNumber { get; init; } = 1;

    [Range(1, int.MaxValue, ErrorMessage = "Page size must be greater than or equal to 1.")]
    public int PageSize { get; init; } = 10;

    public string? Category { get; init; }
    public bool? IsActive { get; init; }
}
