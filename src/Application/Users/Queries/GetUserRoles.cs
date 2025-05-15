using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Common.RequestHandler;
using BaseTemplate.Application.Common.Security;

namespace BaseTemplate.Application.Users.Queries;

[Authorize]
public record GetUserRolesQuery : IRequest<IEnumerable<string>>
{
}

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, IEnumerable<string>>
{
    private readonly IIdentityService _identityService;

    public GetUserRolesQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public async Task<Result<IEnumerable<string>>> HandleAsync(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var res = await _identityService.GetRolesAsync();
        return Result<IEnumerable<string>>.Success(res);
    }
}
