using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.ItemAttributes.Queries.GetItemAttributeById;

public class GetItemAttributeByIdQueryHandler : IRequestHandler<GetItemAttributeByIdQuery, ItemAttributeDto>
{
    private readonly IAppDbContext _context;

    public GetItemAttributeByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

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

        if (itemAttribute == null)
        {
            return Result<ItemAttributeDto>.NotFound($"ItemAttribute with id {request.Id} not found.");
        }

        return Result<ItemAttributeDto>.Success(itemAttribute);
    }
}
