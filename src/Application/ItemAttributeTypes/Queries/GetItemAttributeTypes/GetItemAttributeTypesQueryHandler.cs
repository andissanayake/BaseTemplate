using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.ItemAttributeTypes.Queries.GetItemAttributeTypes;

public class GetItemAttributeTypesQueryHandler : IRequestHandler<GetItemAttributeTypesQuery, List<ItemAttributeTypeBriefDto>>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public GetItemAttributeTypesQueryHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<List<ItemAttributeTypeBriefDto>>> HandleAsync(GetItemAttributeTypesQuery request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        var items = await _context.ItemAttributeType
            .Where(at => at.TenantId == userInfo.TenantId && !at.IsDeleted)
            .OrderByDescending(at => at.Created)
            .Select(at => new ItemAttributeTypeBriefDto
            {
                Id = at.Id,
                Name = at.Name,
                Description = at.Description,
                IsActive = at.IsActive
            })
            .ToListAsync(cancellationToken);

        return Result<List<ItemAttributeTypeBriefDto>>.Success(items);
    }
} 