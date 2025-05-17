using System.ComponentModel.DataAnnotations;

namespace BaseTemplate.Application.Items.Queries.GetItemsWithPagination;

[Authorize]
public record GetItemsWithPaginationQuery : IRequest<PaginatedList<ItemBriefDto>>
{
    public required int TenantId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than or equal to 1.")]
    public int PageNumber { get; init; } = 1;

    [Range(1, int.MaxValue, ErrorMessage = "Page size must be greater than or equal to 1.")]
    public int PageSize { get; init; } = 10;

    public string? Category { get; init; }
    public bool? IsActive { get; init; }
}

public class GetItemsWithPaginationQueryHandler : IRequestHandler<GetItemsWithPaginationQuery, PaginatedList<ItemBriefDto>>
{
    private readonly IUnitOfWorkFactory _factory;

    public GetItemsWithPaginationQueryHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<PaginatedList<ItemBriefDto>>> HandleAsync(GetItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var offset = (request.PageNumber - 1) * request.PageSize;

        const string countSql = @"
            SELECT COUNT(1) 
            FROM item 
            WHERE tenant_id = @TenantId 
            AND (@Category IS NULL OR category = @Category)
            AND (@IsActive IS NULL OR is_active = @IsActive)";

        var totalCount = await uow.QueryFirstOrDefaultAsync<int>(countSql, new 
        { 
            request.TenantId,
            request.Category,
            request.IsActive
        });

        const string dataSql = @"
            SELECT id, tenant_id, name, description, price, is_active, category
            FROM item
            WHERE tenant_id = @TenantId 
            AND (@Category IS NULL OR category = @Category)
            AND (@IsActive IS NULL OR is_active = @IsActive)
            ORDER BY name
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var items = await uow.QueryAsync<ItemBriefDto>(dataSql, new
        {
            request.TenantId,
            request.Category,
            request.IsActive,
            Offset = offset,
            request.PageSize
        });

        return Result<PaginatedList<ItemBriefDto>>.Success(
            new PaginatedList<ItemBriefDto>(items.ToList(), totalCount, request.PageNumber, request.PageSize));
    }
} 