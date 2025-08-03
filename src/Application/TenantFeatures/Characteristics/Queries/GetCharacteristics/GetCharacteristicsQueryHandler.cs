using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Characteristics.Queries.GetCharacteristics;

public class GetCharacteristicsQueryHandler(IAppDbContext context) : IRequestHandler<GetCharacteristicsQuery, List<CharacteristicBriefDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<List<CharacteristicBriefDto>>> HandleAsync(GetCharacteristicsQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.Characteristic.AsNoTracking()
            .Where(ia => ia.CharacteristicTypeId == request.ItemAttributeTypeId)
            .Include(ia => ia.CharacteristicType)
            .OrderByDescending(ia => ia.Created)
            .Select(ia => new CharacteristicBriefDto
            {
                Id = ia.Id,
                Name = ia.Name,
                Code = ia.Code,
                Value = ia.Value,
                IsActive = ia.IsActive,
                ItemAttributeTypeId = ia.CharacteristicTypeId,
                ItemAttributeTypeName = ia.CharacteristicType.Name
            })
            .ToListAsync(cancellationToken);

        return Result<List<CharacteristicBriefDto>>.Success(items);
    }
}
