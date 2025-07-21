namespace BaseTemplate.Application.ItemAttributeTypes.Queries.GetItemAttributeTypeById;

public class GetItemAttributeTypeByIdQueryHandler : IRequestHandler<GetItemAttributeTypeByIdQuery, ItemAttributeTypeDto>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userProfileService;

    public GetItemAttributeTypeByIdQueryHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<ItemAttributeTypeDto>> HandleAsync(GetItemAttributeTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        using var uow = _factory.Create();

        var itemAttributeType = await uow.QuerySingleAsync<ItemAttributeType>(
            "SELECT * FROM item_attribute_type WHERE id = @Id AND tenant_id = @TenantId",
            new { request.Id, TenantId = userInfo.TenantId });

        var dto = new ItemAttributeTypeDto
        {
            Id = itemAttributeType.Id,
            Name = itemAttributeType.Name,
            Description = itemAttributeType.Description,
            IsActive = itemAttributeType.IsActive
        };

        return Result<ItemAttributeTypeDto>.Success(dto);
    }
}
