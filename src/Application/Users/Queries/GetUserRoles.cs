using BaseTemplate.Application.Common.Interfaces;

namespace BaseTemplate.Application.Users.Queries;

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
