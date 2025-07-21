namespace BaseTemplate.Application.ItemAttributes.Queries.GetItemAttributes;

public class GetItemAttributesQueryHandler : IRequestHandler<GetItemAttributesQuery, List<ItemAttributeBriefDto>>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userProfileService;

    public GetItemAttributesQueryHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<List<ItemAttributeBriefDto>>> HandleAsync(GetItemAttributesQuery request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        using var uow = _factory.Create();

        var sql = @"
            SELECT ia.*, iat.name as item_attribute_type_name 
            FROM item_attribute ia 
            LEFT JOIN item_attribute_type iat ON ia.item_attribute_type_id = iat.id 
            WHERE ia.tenant_id = @TenantId
            AND ia.item_attribute_type_id = @ItemAttributeTypeId
            AND ia.is_deleted != 1
            ORDER BY ia.created DESC";

        var parameters = new
        {
            TenantId = userInfo.TenantId,
            ItemAttributeTypeId = request.ItemAttributeTypeId
        };

        var items = await uow.QueryAsync<dynamic>(sql, parameters);

        var dtos = items.Select(a => new ItemAttributeBriefDto
        {
            Id = a.id,
            Name = a.name,
            Code = a.code,
            Value = a.value,
            IsActive = a.is_active,
            ItemAttributeTypeId = a.item_attribute_type_id,
            ItemAttributeTypeName = a.item_attribute_type_name
        }).ToList();

        return Result<List<ItemAttributeBriefDto>>.Success(dtos);
    }
}
