namespace BaseTemplate.Application.Tenants.Commands.CreateTenant;

[Authorize]
public record CreateTenantCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
}

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, int>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUser _user;
    public CreateTenantCommandHandler(IUnitOfWorkFactory factory, IUser user)
    {
        _factory = factory;
        _user = user;
    }

    public async Task<Result<int>> HandleAsync(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var exEntity = await uow.QueryFirstOrDefaultAsync<Tenant>("select * from tenant where owner_identifier=@Identifier", new { _user.Identifier });
        if (exEntity != null)
        {
            return Result<int>.Success(exEntity.Id, "already have tenant single user can have only one tenant.");
        }
        else
        {
            var entity = new Tenant
            {
                Name = request.Name,
                Address = request.Address,
                OwnerIdentifier = _user.Identifier!
            };
            var data = await uow.InsertAsync(entity);
            return Result<int>.Success(data);
        }
    }
}
