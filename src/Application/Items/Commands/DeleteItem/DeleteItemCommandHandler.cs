namespace BaseTemplate.Application.Items.Commands.DeleteItem;

public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userProfileService;

    public DeleteItemCommandHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        // Get user profile to get tenant ID
        var userProfile = await _userProfileService.GetUserProfileAsync();

        using var uow = _factory.Create();
        var entity = await uow.QueryFirstOrDefaultAsync<Item>("select * from item where Id = @Id and tenant_id = @TenantId", new { request.Id, TenantId = userProfile.TenantId });

        if (entity is null)
        {
            return Result<bool>.NotFound($"Item with id {request.Id} not found.");
        }

        entity.IsActive = false;
        await uow.UpdateAsync(entity);

        return Result<bool>.Success(true);
    }
}
