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
            .Join(_context.ItemAttributeType,
                  ia => ia.ItemAttributeTypeId,
                  iat => iat.Id,
                  (ia, iat) => new { ia, iat })
            .Select(x => new ItemAttributeDto
            {
                Id = x.ia.Id,
                Name = x.ia.Name,
                Code = x.ia.Code,
                Value = x.ia.Value,
                IsActive = x.ia.IsActive,
                ItemAttributeTypeId = x.ia.ItemAttributeTypeId,
                ItemAttributeTypeName = x.iat.Name
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (itemAttribute == null)
        {
            return Result<ItemAttributeDto>.NotFound($"ItemAttribute with id {request.Id} not found.");
        }

        return Result<ItemAttributeDto>.Success(itemAttribute);
    }
}
