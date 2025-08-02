using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.ItemAttributeTypes.Queries.GetItemAttributeTypeById;

public class GetItemAttributeTypeByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetItemAttributeTypeByIdQuery, ItemAttributeTypeDto>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<ItemAttributeTypeDto>> HandleAsync(GetItemAttributeTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var itemAttributeType = await _context.ItemAttributeType
            .SingleAsync(iat => iat.Id == request.Id, cancellationToken);

        var dto = new ItemAttributeTypeDto
        {
            Id = itemAttributeType.Id,
            Name = itemAttributeType.Name,
            Description = itemAttributeType.Description,
            IsActive = itemAttributeType.IsActive
        };
        return Result<ItemAttributeTypeDto>.Success(dto);
    }
}
