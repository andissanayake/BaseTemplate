using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.ItemAttributes.Queries.GetItemAttributeById;

public class GetItemAttributeByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetItemAttributeByIdQuery, ItemAttributeDto>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<ItemAttributeDto>> HandleAsync(GetItemAttributeByIdQuery request, CancellationToken cancellationToken)
    {
        var itemAttribute = await _context.ItemAttribute
            .Where(ia => ia.Id == request.Id)
            .Include(ia => ia.ItemAttributeType)
            .Select(ia => new ItemAttributeDto
            {
                Id = ia.Id,
                Name = ia.Name,
                Code = ia.Code,
                Value = ia.Value,
                IsActive = ia.IsActive,
                ItemAttributeTypeId = ia.ItemAttributeTypeId,
                ItemAttributeTypeName = ia.ItemAttributeType!.Name
            })
            .FirstOrDefaultAsync(cancellationToken);

        return itemAttribute == null
            ? Result<ItemAttributeDto>.NotFound($"ItemAttribute with id {request.Id} not found.")
            : Result<ItemAttributeDto>.Success(itemAttribute);
    }
}
