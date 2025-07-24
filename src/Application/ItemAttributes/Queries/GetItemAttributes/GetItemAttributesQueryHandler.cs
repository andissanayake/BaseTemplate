using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.ItemAttributes.Queries.GetItemAttributes;

public class GetItemAttributesQueryHandler : IRequestHandler<GetItemAttributesQuery, List<ItemAttributeBriefDto>>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public GetItemAttributesQueryHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<List<ItemAttributeBriefDto>>> HandleAsync(GetItemAttributesQuery request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        var items = await _context.ItemAttribute
            .Where(ia => ia.TenantId == userInfo.TenantId && ia.ItemAttributeTypeId == request.ItemAttributeTypeId && !ia.IsDeleted)
            .Join(_context.ItemAttributeType,
                  ia => ia.ItemAttributeTypeId,
                  iat => iat.Id,
                  (ia, iat) => new { ia, iat })
            .OrderByDescending(x => x.ia.Created)
            .Select(x => new ItemAttributeBriefDto
            {
                Id = x.ia.Id,
                Name = x.ia.Name,
                Code = x.ia.Code,
                Value = x.ia.Value,
                IsActive = x.ia.IsActive,
                ItemAttributeTypeId = x.ia.ItemAttributeTypeId,
                ItemAttributeTypeName = x.iat.Name
            })
            .ToListAsync(cancellationToken);

        return Result<List<ItemAttributeBriefDto>>.Success(items);
    }
}
