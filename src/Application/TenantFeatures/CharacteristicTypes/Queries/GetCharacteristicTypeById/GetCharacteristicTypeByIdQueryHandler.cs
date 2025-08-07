using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.CharacteristicTypes.Queries.GetCharacteristicTypeById;

public class GetCharacteristicTypeByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetCharacteristicTypeByIdQuery, CharacteristicTypeDto>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CharacteristicTypeDto>> HandleAsync(GetCharacteristicTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var characteristicType = await _context.CharacteristicType
            .SingleAsync(iat => iat.Id == request.Id, cancellationToken);

        var dto = new CharacteristicTypeDto
        {
            Id = characteristicType.Id,
            Name = characteristicType.Name,
            Description = characteristicType.Description,
            IsActive = characteristicType.IsActive
        };
        return Result<CharacteristicTypeDto>.Success(dto);
    }
}
