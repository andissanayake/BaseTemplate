namespace BaseTemplate.Application.ItemAttributes.Commands.DeleteItemAttribute;

public class DeleteItemAttributeCommandHandler : IRequestHandler<DeleteItemAttributeCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userProfileService;

    public DeleteItemAttributeCommandHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(DeleteItemAttributeCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        using var uow = _factory.Create();

        var itemAttribute = await uow.QuerySingleAsync<ItemAttribute>(
            "SELECT * FROM item_attribute WHERE id = @Id AND tenant_id = @TenantId",
            new { request.Id, TenantId = userInfo.TenantId });

        itemAttribute.IsActive = false;
        await uow.UpdateAsync(itemAttribute);
        return Result<bool>.Success(true);
    }
}
