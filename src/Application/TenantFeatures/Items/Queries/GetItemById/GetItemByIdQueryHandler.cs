using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Items.Queries.GetItemById;

public class GetItemByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetItemByIdQuery, ItemDto>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<ItemDto>> HandleAsync(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Item.AsNoTracking()
            .Include(i => i.Specification)
                .ThenInclude(s => s.ParentSpecification)
            .Include(i => i.ItemCharacteristicTypeList)
                .ThenInclude(ict => ict.CharacteristicType)
            .SingleAsync(i => i.Id == request.Id, cancellationToken);

        var specificationFullPath = entity.Specification?.FullPath ?? string.Empty;

        var characteristicTypes = entity.ItemCharacteristicTypeList.Select(ict => new ItemCharacteristicTypeDto
        {
            Id = ict.Id,
            CharacteristicTypeId = ict.CharacteristicTypeId,
            CharacteristicTypeName = ict.CharacteristicType.Name,
            CharacteristicTypeDescription = ict.CharacteristicType.Description
        }).ToList();

        var itemDto = new ItemDto
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            IsActive = entity.IsActive,
            Category = entity.Category,
            SpecificationId = entity.SpecificationId,
            SpecificationFullPath = specificationFullPath,
            CharacteristicTypes = characteristicTypes
        };

        return Result<ItemDto>.Success(itemDto);
    }
}
