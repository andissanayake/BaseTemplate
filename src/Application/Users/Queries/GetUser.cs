using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Common.RequestHandler;
using BaseTemplate.Application.Common.Security;
using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Application.Users.Queries;

[Authorize]
public record GetUserQuery : IRequest<GetUserResponse>
{
}
public record GetUserResponse
{
    public IEnumerable<string> Roles { get; set; } = new List<string>();
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
        var user = await uow.QueryFirstOrDefaultAsync<AppUser>(@"
            SELECT *
            FROM app_user
            WHERE identifier = @Identifier", new { _user.Identifier });
        if (user == null)
        {
            await uow.InsertAsync(new AppUser() { Identifier = _user.Identifier!, Name = _user.Name, Email = _user.Email });
        }
        else
        {
            user.Name = _user.Name;
            user.Email = _user.Email;
            await uow.UpdateAsync(user);
        }
        var roles = await uow.QueryAsync<string>("select role from user_role where user_identifier = @Identifier", new { _user.Identifier });
        return Result<GetUserResponse>.Success(new GetUserResponse() { Roles = roles });
    }
}
