namespace BaseTemplate.Application.ItemAttributes.Queries.GetItemAttributeById;

public class GetItemAttributeByIdQueryHandler : IRequestHandler<GetItemAttributeByIdQuery, ItemAttributeDto>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserProfileService _userProfileService;

    public GetItemAttributeByIdQueryHandler(IUnitOfWorkFactory factory, IUserProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<ItemAttributeDto>> HandleAsync(GetItemAttributeByIdQuery request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        using var uow = _factory.Create();

        var sql = @"
            SELECT ia.*, iat.name as item_attribute_type_name 
            FROM item_attribute ia 
            LEFT JOIN item_attribute_type iat ON ia.item_attribute_type_id = iat.id 
            WHERE ia.id = @Id AND ia.tenant_id = @TenantId
            AND ia.is_deleted = FALSE
        ";

        var itemAttribute = await uow.QuerySingleAsync<dynamic>(sql, new { request.Id, TenantId = userInfo.TenantId });

        var dto = new ItemAttributeDto
        {
            Id = itemAttribute.id,
            Name = itemAttribute.name,
            Code = itemAttribute.code,
            Value = itemAttribute.value,
            IsActive = itemAttribute.is_active,
            ItemAttributeTypeId = itemAttribute.item_attribute_type_id,
            ItemAttributeTypeName = itemAttribute.item_attribute_type_name
        };

        return Result<ItemAttributeDto>.Success(dto);
    }
}
