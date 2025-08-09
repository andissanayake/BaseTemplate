using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Items.Queries.GetItemVariantByItemId;

public class GetItemVariantsByItemIdQueryHandler(IAppDbContext context) : IRequestHandler<GetItemVariantsByItemIdQuery, List<ItemVariantDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<List<ItemVariantDto>>> HandleAsync(GetItemVariantsByItemIdQuery request, CancellationToken cancellationToken)
    {
        // Verify item exists
        var itemExists = await _context.Item
            .AnyAsync(i => i.Id == request.ItemId, cancellationToken);

        if (!itemExists)
        {
            return Result<List<ItemVariantDto>>.NotFound($"Item with ID {request.ItemId} not found");
        }

        // Get all variants for the item with their characteristics and item details
        var variants = await _context.ItemVariant
            .AsNoTracking()
            .Include(iv => iv.Item)
                .ThenInclude(i => i.Specification)
            .Include(iv => iv.ItemVariantCharacteristicList)
                .ThenInclude(ivc => ivc.Characteristic)
                    .ThenInclude(c => c.CharacteristicType)
            .Where(iv => iv.ItemId == request.ItemId)
            .OrderBy(iv => iv.Code)
            .ToListAsync(cancellationToken);

        var variantDtos = variants.Select(variant => new ItemVariantDto
        {
            Id = variant.Id,
            ItemId = variant.ItemId,
            Code = variant.Code,
            Price = variant.Price,
            Item = new ItemDetailDto
            {
                Id = variant.Item.Id,
                Name = variant.Item.Name,
                Code = variant.Item.Code,
                Description = variant.Item.Description,
                IsActive = variant.Item.IsActive,
                Tags = variant.Item.Tags,
                SpecificationId = variant.Item.SpecificationId,
                SpecificationFullPath = variant.Item.Specification?.FullPath ?? string.Empty,
                HasVariant = variant.Item.HasVariant
            },
            Characteristics = variant.ItemVariantCharacteristicList.Select(ivc => new ItemVariantCharacteristicDto
            {
                Id = ivc.Id,
                CharacteristicId = ivc.CharacteristicId,
                CharacteristicName = ivc.Characteristic.Name,
                CharacteristicCode = ivc.Characteristic.Code,
                CharacteristicValue = ivc.Characteristic.Value,
                CharacteristicTypeId = ivc.Characteristic.CharacteristicTypeId,
                CharacteristicTypeName = ivc.Characteristic.CharacteristicType.Name
            }).OrderBy(c => c.CharacteristicTypeName).ToList()
        }).ToList();

        return Result<List<ItemVariantDto>>.Success(variantDtos);
    }
}