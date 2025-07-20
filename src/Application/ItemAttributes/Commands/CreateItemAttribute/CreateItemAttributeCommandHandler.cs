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
