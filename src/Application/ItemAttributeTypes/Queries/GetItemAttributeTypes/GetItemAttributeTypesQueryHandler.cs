namespace BaseTemplate.Application.ItemAttributeTypes.Queries.GetItemAttributeTypes;

public class GetItemAttributeTypesQueryHandler : IRequestHandler<GetItemAttributeTypesQuery, List<ItemAttributeTypeBriefDto>>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userProfileService;

    public GetItemAttributeTypesQueryHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<List<ItemAttributeTypeBriefDto>>> HandleAsync(GetItemAttributeTypesQuery request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        using var uow = _factory.Create();

        var sql = @"
            SELECT * FROM item_attribute_type 
            WHERE tenant_id = @TenantId
            AND (@SearchTerm IS NULL OR name ILIKE @SearchTerm OR description ILIKE @SearchTerm)
            AND (@IsActive IS NULL OR is_active = @IsActive)
            ORDER BY created DESC";

        var parameters = new 
        { 
            TenantId = userInfo.TenantId,
            SearchTerm = string.IsNullOrWhiteSpace(request.SearchTerm) ? null : $"%{request.SearchTerm}%",
            IsActive = request.IsActive
        };

        var items = await uow.QueryAsync<ItemAttributeType>(sql, parameters);
        
        var dtos = items.Select(at => new ItemAttributeTypeBriefDto
        {
            Id = at.Id,
            Name = at.Name,
            Description = at.Description,
            IsActive = at.IsActive,
            Created = at.Created,
            CreatedBy = at.CreatedBy
        }).ToList();
        
        return Result<List<ItemAttributeTypeBriefDto>>.Success(dtos);
    }
} 