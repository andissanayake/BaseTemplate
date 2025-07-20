namespace BaseTemplate.Application.ItemAttributes.Commands.CreateItemAttribute;

public class CreateItemAttributeCommandHandler : IRequestHandler<CreateItemAttributeCommand, int>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userProfileService;

    public CreateItemAttributeCommandHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<int>> HandleAsync(CreateItemAttributeCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        using var uow = _factory.Create();

        // Check if code already exists for this tenant
        var existingAttribute = await uow.QueryFirstOrDefaultAsync<ItemAttribute>(
            "SELECT * FROM item_attribute WHERE code = @Code AND tenant_id = @TenantId",
            new { request.Code, TenantId = userInfo.TenantId });

        if (existingAttribute != null)
        {
            return Result<int>.Validation("Code must be unique within the tenant", []);
        }

        var itemAttribute = new ItemAttribute
        {
            Name = request.Name,
            Code = request.Code,
            Value = request.Value,
            ItemAttributeTypeId = request.ItemAttributeTypeId,
            TenantId = userInfo.TenantId,
            IsActive = true
        };

        await uow.InsertAsync(itemAttribute);
        return Result<int>.Success(itemAttribute.Id);
    }
}
