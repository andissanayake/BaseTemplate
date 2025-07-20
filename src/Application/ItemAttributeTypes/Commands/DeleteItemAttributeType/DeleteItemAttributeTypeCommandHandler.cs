namespace BaseTemplate.Application.ItemAttributeTypes.Commands.DeleteItemAttributeType;

public class DeleteItemAttributeTypeCommandHandler : IRequestHandler<DeleteItemAttributeTypeCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userProfileService;

    public DeleteItemAttributeTypeCommandHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(DeleteItemAttributeTypeCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        using var uow = _factory.Create();

        var itemAttributeType = await uow.QuerySingleAsync<ItemAttributeType>(
            "SELECT * FROM item_attribute_type WHERE id = @Id AND tenant_id = @TenantId",
            new { request.Id, TenantId = userInfo.TenantId });

        itemAttributeType.IsActive = false;
        await uow.UpdateAsync(itemAttributeType);
        return Result<bool>.Success(true);
    }
}
