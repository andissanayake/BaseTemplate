using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.ItemAttributeTypes.Queries.GetItemAttributeTypes;

public class GetItemAttributeTypesQueryHandler : IRequestHandler<GetItemAttributeTypesQuery, List<ItemAttributeTypeBriefDto>>
{
    private readonly IAppDbContext _context;

    public GetItemAttributeTypesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ItemAttributeTypeBriefDto>>> HandleAsync(GetItemAttributeTypesQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.ItemAttributeType
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