using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Items.Queries.GetItemsWithPagination;

public class GetItemsWithPaginationQueryHandler(IAppDbContext context) : IRequestHandler<GetItemsWithPaginationQuery, PaginatedList<ItemBriefDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PaginatedList<ItemBriefDto>>> HandleAsync(GetItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Item.AsNoTracking()
            .Include(i => i.Specification)
                .ThenInclude(s => s.ParentSpecification)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Tags))
            query = query.Where(i => i.Tags == request.Tags);

        if (request.IsActive.HasValue)
            query = query.Where(i => i.IsActive == request.IsActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(i => i.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var itemDtos = items.Select(i => new ItemBriefDto
        {
            Id = i.Id,
            TenantId = i.TenantId,
            Name = i.Name,
            Description = i.Description,
            IsActive = i.IsActive,
            Tags = i.Tags,
            SpecificationId = i.SpecificationId,
            SpecificationFullPath = i.Specification?.FullPath ?? string.Empty
        }).ToList();

        return Result<PaginatedList<ItemBriefDto>>.Success(
            new PaginatedList<ItemBriefDto>(itemDtos, totalCount, request.PageNumber, request.PageSize));
    }
}
