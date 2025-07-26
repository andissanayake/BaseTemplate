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
            .Include(ia => ia.ItemAttributeType)
            .OrderByDescending(ia => ia.Created)
            .Select(ia => new ItemAttributeBriefDto
            {
                Id = ia.Id,
                Name = ia.Name,
                Code = ia.Code,
                Value = ia.Value,
                IsActive = ia.IsActive,
                ItemAttributeTypeId = ia.ItemAttributeTypeId,
                ItemAttributeTypeName = ia.ItemAttributeType.Name
            })
            .ToListAsync(cancellationToken);

        return Result<List<ItemAttributeBriefDto>>.Success(items);
    }
}
