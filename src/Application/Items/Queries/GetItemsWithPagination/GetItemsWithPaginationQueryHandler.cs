namespace BaseTemplate.Application.Items.Queries.GetItemsWithPagination;

public class GetItemsWithPaginationQueryHandler : IRequestHandler<GetItemsWithPaginationQuery, PaginatedList<ItemBriefDto>>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userProfileService;

    public GetItemsWithPaginationQueryHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<PaginatedList<ItemBriefDto>>> HandleAsync(GetItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        using var uow = _factory.Create();
        var offset = (request.PageNumber - 1) * request.PageSize;

        const string countSql = @"
            SELECT COUNT(1) 
            FROM item 
            WHERE tenant_id = @TenantId 
            AND (@Category IS NULL OR category = @Category)
            AND (@IsActive IS NULL OR is_active = @IsActive)
            AND is_deleted = FALSE
        ";

        var totalCount = await uow.QueryFirstOrDefaultAsync<int>(countSql, new
        {
            userInfo.TenantId,
            request.Category,
            request.IsActive
        });

        const string dataSql = @"
            SELECT id, tenant_id, name, description, price, is_active, category
            FROM item
            WHERE tenant_id = @TenantId 
            AND (@Category IS NULL OR category = @Category)
            AND (@IsActive IS NULL OR is_active = @IsActive)
            AND is_deleted = FALSE
            ORDER BY name
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var items = await uow.QueryAsync<ItemBriefDto>(dataSql, new
        {
            userInfo.TenantId,
            request.Category,
            request.IsActive,
            Offset = offset,
            request.PageSize
        });

        return Result<PaginatedList<ItemBriefDto>>.Success(
            new PaginatedList<ItemBriefDto>(items.ToList(), totalCount, request.PageNumber, request.PageSize));
    }
}
