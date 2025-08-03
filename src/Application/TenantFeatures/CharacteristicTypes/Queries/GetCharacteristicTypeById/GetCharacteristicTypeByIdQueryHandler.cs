using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.CharacteristicTypes.Queries.GetCharacteristicTypeById;

public class GetCharacteristicTypeByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetCharacteristicTypeByIdQuery, CharacteristicTypeDto>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CharacteristicTypeDto>> HandleAsync(GetCharacteristicTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var itemAttributeType = await _context.CharacteristicType
            .SingleAsync(iat => iat.Id == request.Id, cancellationToken);

        var dto = new CharacteristicTypeDto
        {
            Id = itemAttributeType.Id,
            Name = itemAttributeType.Name,
            Description = itemAttributeType.Description,
            IsActive = itemAttributeType.IsActive
        };
        return Result<CharacteristicTypeDto>.Success(dto);
    }
}
