namespace BaseTemplate.Application.Items.Commands.CreateItem;

public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, int>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userProfileService;
    public CreateItemCommandHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<int>> HandleAsync(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        using var uow = _factory.Create();

        var entity = new Item
        {
            TenantId = userInfo.TenantId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            IsActive = true
        };

        await uow.InsertAsync(entity);
        return Result<int>.Success(entity.Id);
    }
}
