namespace BaseTemplate.Application.ItemAttributeTypes.Queries.GetItemAttributeTypes;

public class GetItemAttributeTypesQueryHandler : IRequestHandler<GetItemAttributeTypesQuery, List<ItemAttributeTypeBriefDto>>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserProfileService _userProfileService;

    public GetItemAttributeTypesQueryHandler(IUnitOfWorkFactory factory, IUserProfileService userProfileService)
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
            AND is_deleted = FALSE
            ORDER BY created DESC";

        var parameters = new 
        { 
            TenantId = userInfo.TenantId
        };

        var items = await uow.QueryAsync<ItemAttributeType>(sql, parameters);
        
        var dtos = items.Select(at => new ItemAttributeTypeBriefDto
        {
            Id = at.Id,
            Name = at.Name,
            Description = at.Description,
            IsActive = at.IsActive
        }).ToList();
        
        return Result<List<ItemAttributeTypeBriefDto>>.Success(dtos);
    }
} 