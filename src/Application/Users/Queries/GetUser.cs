namespace BaseTemplate.Application.Users.Queries;

[Authorize]
public record GetUserQuery : IRequest<GetUserResponse>
{
}
public record GetUserResponse
{
    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }
}

public record UserWithTenantInfo
{
    public int Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }
}

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserResponse>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUser _user;

    public GetUserQueryHandler(IUnitOfWorkFactory factory, IUser user)
    {
        _factory = factory;
        _user = user;
    }
    public async Task<Result<GetUserResponse>> HandleAsync(GetUserQuery request, CancellationToken cancellationToken)
    {
        var uow = _factory.Create();
        //Ensure the user exists in the database
        var userInfo = await uow.QueryFirstOrDefaultAsync<UserWithTenantInfo>(@"
            SELECT u.Id, u.Identifier, u.Name, u.Email, t.Id as TenantId, t.Name as TenantName
            FROM app_user u
            LEFT JOIN Tenant t ON u.identifier = t.owner_identifier
            WHERE u.identifier = @Identifier", new { _user.Identifier });

        if (userInfo == null)
        {
            var newAppUser = new AppUser() { Identifier = _user.Identifier!, Name = _user.Name, Email = _user.Email };
            await uow.InsertAsync(newAppUser);
            return Result<GetUserResponse>.Success(new GetUserResponse() { Roles = new List<string>() });
        }
        else
        {
            // Load minimal AppUser for update
            var existingUser = new AppUser { Id = userInfo.Id, Identifier = userInfo.Identifier };
            bool changed = false;
            if (userInfo.Name != _user.Name)
            {
                existingUser.Name = _user.Name;
                changed = true;
            }
            if (userInfo.Email != _user.Email)
            {
                existingUser.Email = _user.Email;
                changed = true;
            }

            if (changed)
            {
                await uow.UpdateAsync(existingUser);
            }
        }
        var roles = await uow.QueryAsync<string>("select role from user_role where user_identifier = @Identifier", new { _user.Identifier });
        return Result<GetUserResponse>.Success(new GetUserResponse() { Roles = roles, TenantId = userInfo?.TenantId, TenantName = userInfo?.TenantName });
    }
}
