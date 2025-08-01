using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Items.Queries.GetItemsWithPagination;

public class GetItemsWithPaginationQueryHandler : IRequestHandler<GetItemsWithPaginationQuery, PaginatedList<ItemBriefDto>>
{
    private readonly IAppDbContext _context;

    public GetItemsWithPaginationQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<ItemBriefDto>>> HandleAsync(GetItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Item.AsQueryable();

        if (!string.IsNullOrEmpty(request.Category))
            query = query.Where(i => i.Category == request.Category);

        if (request.IsActive.HasValue)
            query = query.Where(i => i.IsActive == request.IsActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(i => i.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(i => new ItemBriefDto
            {
                Id = i.Id,
                TenantId = i.TenantId,
                Name = i.Name,
                Description = i.Description,
                Price = i.Price,
                IsActive = i.IsActive,
                Category = i.Category
            })
            .ToListAsync(cancellationToken);

        return Result<PaginatedList<ItemBriefDto>>.Success(
            new PaginatedList<ItemBriefDto>(items, totalCount, request.PageNumber, request.PageSize));
    }
}
