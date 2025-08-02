using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Items.Queries.GetItemById;

public class GetItemByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetItemByIdQuery, ItemDto>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<ItemDto>> HandleAsync(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Item
            .SingleAsync(i => i.Id == request.Id, cancellationToken);

        var itemDto = new ItemDto
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            IsActive = entity.IsActive,
            Category = entity.Category
        };

        return Result<ItemDto>.Success(itemDto);
    }
}
