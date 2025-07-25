using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.ItemAttributes.Queries.GetItemAttributeById;

public class GetItemAttributeByIdQueryHandler : IRequestHandler<GetItemAttributeByIdQuery, ItemAttributeDto>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public GetItemAttributeByIdQueryHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<ItemAttributeDto>> HandleAsync(GetItemAttributeByIdQuery request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        var itemAttribute = await _context.ItemAttribute
            .Where(ia => ia.Id == request.Id && ia.TenantId == userInfo.TenantId && !ia.IsDeleted)
            .Include(ia => ia.ItemAttributeType)
            .Select(ia => new ItemAttributeDto
            {
                Id = ia.Id,
                Name = ia.Name,
                Code = ia.Code,
                Value = ia.Value,
                IsActive = ia.IsActive,
                ItemAttributeTypeId = ia.ItemAttributeTypeId,
                ItemAttributeTypeName = ia.ItemAttributeType.Name
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (itemAttribute == null)
        {
            return Result<ItemAttributeDto>.NotFound($"ItemAttribute with id {request.Id} not found.");
        }

        return Result<ItemAttributeDto>.Success(itemAttribute);
    }
}
