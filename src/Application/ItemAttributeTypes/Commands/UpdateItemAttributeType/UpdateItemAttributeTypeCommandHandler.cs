namespace BaseTemplate.Application.ItemAttributeTypes.Commands.UpdateItemAttributeType;

public class UpdateItemAttributeTypeCommandHandler : IRequestHandler<UpdateItemAttributeTypeCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userProfileService;

    public UpdateItemAttributeTypeCommandHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(UpdateItemAttributeTypeCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        using var uow = _factory.Create();

        var itemAttributeType = await uow.QuerySingleAsync<ItemAttributeType>(
            "SELECT * FROM item_attribute_type WHERE id = @Id AND tenant_id = @TenantId",
            new { request.Id, TenantId = userInfo.TenantId });

        itemAttributeType.Name = request.Name;
        itemAttributeType.Description = request.Description;
        itemAttributeType.IsActive = true;

        await uow.UpdateAsync(itemAttributeType);
        return Result<bool>.Success(true);
    }
}
