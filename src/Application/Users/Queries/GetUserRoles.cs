using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Security;

namespace BaseTemplate.Application.Users.Queries;

[Authorize]
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

    public async Task<IEnumerable<string>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.GetRolesAsync(_user.Id!);
    }
}
