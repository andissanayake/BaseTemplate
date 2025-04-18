using BaseTemplate.Application.Common.Interfaces;

namespace BaseTemplate.Application.Users.Queries;

public record GetUserRolesQuery : IRequest<IEnumerable<string>>
{
}

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, IEnumerable<string>>
{
    private readonly IIdentityService _identityService;
    private readonly IUser _user;

    public GetUserRolesQueryHandler(IIdentityService identityService, IUser user)
    {
        _identityService = identityService;
        _user = user;
    }

    public async Task<Result<IEnumerable<string>>> AuthorizeAsync(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        // [Authorize]
        return Result<IEnumerable<string>>.Success(new List<string>());
    }
    public async Task<Result<IEnumerable<string>>> HandleAsync(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var res = await _identityService.GetRolesAsync(_user.Id!);
        return Result<IEnumerable<string>>.Success(res);
    }
}
