namespace BaseTemplate.Application.Items.Commands.DeleteItem;

public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserProfileService _userProfileService;

    public DeleteItemCommandHandler(IUnitOfWorkFactory factory, IUserProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        // Get user profile to get tenant ID
        var userProfile = await _userProfileService.GetUserProfileAsync();
        if (userProfile?.TenantId == null)
        {
            return Result<bool>.Forbidden("User is not associated with any tenant.");
        }

        var tenantId = userProfile.TenantId.Value;

        using var uow = _factory.Create();
        var entity = await uow.QueryFirstOrDefaultAsync<Item>("select * from item where Id = @Id and tenant_id = @TenantId", new { request.Id, TenantId = tenantId });

        if (entity is null)
        {
            return Result<bool>.NotFound($"Item with id {request.Id} not found.");
        }

        // Soft delete by setting IsActive to false
        entity.IsActive = false;
        await uow.UpdateAsync(entity);

        return Result<bool>.Success(true);
    }
} 