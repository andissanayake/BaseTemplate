namespace BaseTemplate.Application.ItemAttributeTypes.Commands.CreateItemAttributeType;

public class CreateItemAttributeTypeCommandHandler : IRequestHandler<CreateItemAttributeTypeCommand, int>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userProfileService;

    public CreateItemAttributeTypeCommandHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<int>> HandleAsync(CreateItemAttributeTypeCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        using var uow = _factory.Create();

        var itemAttributeType = new ItemAttributeType
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
            TenantId = userInfo.TenantId
        };

        await uow.InsertAsync(itemAttributeType);
        return Result<int>.Success(itemAttributeType.Id);
    }
} 