using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.ItemAttributeTypes.Queries.GetItemAttributeTypes;

public class GetItemAttributeTypesQueryHandler(IAppDbContext context) : IRequestHandler<GetItemAttributeTypesQuery, List<ItemAttributeTypeBriefDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<List<ItemAttributeTypeBriefDto>>> HandleAsync(GetItemAttributeTypesQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.ItemAttributeType
            .Where(at => at.IsActive)
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
