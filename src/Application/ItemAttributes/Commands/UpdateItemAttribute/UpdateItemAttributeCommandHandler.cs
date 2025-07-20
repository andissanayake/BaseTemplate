namespace BaseTemplate.Application.ItemAttributes.Commands.UpdateItemAttribute;

public class UpdateItemAttributeCommandHandler : IRequestHandler<UpdateItemAttributeCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userProfileService;

    public UpdateItemAttributeCommandHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(UpdateItemAttributeCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        using var uow = _factory.Create();

        var itemAttribute = await uow.QuerySingleAsync<ItemAttribute>(
            "SELECT * FROM item_attribute WHERE id = @Id AND tenant_id = @TenantId",
            new { request.Id, TenantId = userInfo.TenantId });

        itemAttribute.Name = request.Name;
        itemAttribute.Code = request.Code;
        itemAttribute.Value = request.Value;
        itemAttribute.IsActive = request.IsActive;

        await uow.UpdateAsync(itemAttribute);
        return Result<bool>.Success(true);
    }
}
