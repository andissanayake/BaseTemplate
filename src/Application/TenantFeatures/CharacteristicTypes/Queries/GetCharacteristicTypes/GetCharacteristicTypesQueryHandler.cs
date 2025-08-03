using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.CharacteristicTypes.Queries.GetCharacteristicTypes;

public class GetCharacteristicTypesQueryHandler(IAppDbContext context) : IRequestHandler<GetCharacteristicTypesQuery, List<CharacteristicTypeBriefDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<List<CharacteristicTypeBriefDto>>> HandleAsync(GetCharacteristicTypesQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.CharacteristicType
            .OrderByDescending(at => at.Created)
            .Select(at => new CharacteristicTypeBriefDto
            {
                Id = at.Id,
                Name = at.Name,
                Description = at.Description,
                IsActive = at.IsActive
            })
            .ToListAsync(cancellationToken);

        return Result<List<CharacteristicTypeBriefDto>>.Success(items);
    }
}
