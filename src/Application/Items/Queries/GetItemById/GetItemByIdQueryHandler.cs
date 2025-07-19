namespace BaseTemplate.Application.Items.Queries.GetItemById;

public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, ItemDto>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userProfileService;

    public GetItemByIdQueryHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<ItemDto>> HandleAsync(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        using var uow = _factory.Create();
        var entity = await uow.QuerySingleAsync<Item>("select * from item where id = @Id and tenant_id = @TenantId", new { request.Id, userInfo.TenantId });

        var itemDto = new ItemDto
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            IsActive = entity.IsActive,
            Category = entity.Category
        };

        return Result<ItemDto>.Success(itemDto);
    }
}
