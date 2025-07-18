namespace BaseTemplate.Application.Items.Commands.UpdateItem;

public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userProfileService;

    public UpdateItemCommandHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        using var uow = _factory.Create();
        var entity = await uow.QuerySingleAsync<Item>("select * from item where Id = @Id and tenant_id = @TenantId", new { request.Id, userInfo.TenantId });

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.Category = request.Category;
        entity.IsActive = request.IsActive;

        await uow.UpdateAsync(entity);
        return Result<bool>.Success(true);
    }
}
