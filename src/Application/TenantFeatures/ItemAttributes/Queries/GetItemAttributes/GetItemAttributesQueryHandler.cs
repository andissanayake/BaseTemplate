using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.ItemAttributes.Queries.GetItemAttributes;

public class GetItemAttributesQueryHandler(IAppDbContext context) : IRequestHandler<GetItemAttributesQuery, List<ItemAttributeBriefDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<List<ItemAttributeBriefDto>>> HandleAsync(GetItemAttributesQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.ItemAttribute.AsNoTracking()
            .Where(ia => ia.ItemAttributeTypeId == request.ItemAttributeTypeId)
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
