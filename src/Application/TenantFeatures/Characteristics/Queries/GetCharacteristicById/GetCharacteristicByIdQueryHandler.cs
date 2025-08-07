using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Characteristics.Queries.GetCharacteristicById;

public class GetCharacteristicByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetICharacteristicByIdQuery, CharacteristicDto>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CharacteristicDto>> HandleAsync(GetICharacteristicByIdQuery request, CancellationToken cancellationToken)
    {
        var characteristic = await _context.Characteristic.AsNoTracking()
            .Where(ia => ia.Id == request.Id)
            .Include(ia => ia.CharacteristicType)
            .Select(ia => new CharacteristicDto
            {
                Id = ia.Id,
                Name = ia.Name,
                Code = ia.Code,
                Value = ia.Value,
                IsActive = ia.IsActive,
                            CharacteristicTypeId = ia.CharacteristicTypeId,
            CharacteristicTypeName = ia.CharacteristicType!.Name
            }).SingleAsync(cancellationToken);

        return Result<CharacteristicDto>.Success(characteristic);
    }
}
