using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.CharacteristicTypes.Commands.UpdateCharacteristicType;

public class UpdateCharacteristicTypeCommandHandler(IAppDbContext context) : IRequestHandler<UpdateCharacteristicTypeCommand, bool>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<bool>> HandleAsync(UpdateCharacteristicTypeCommand request, CancellationToken cancellationToken)
    {

        var itemAttributeType = await _context.CharacteristicType
            .SingleAsync(t => t.Id == request.Id, cancellationToken);

        itemAttributeType.Name = request.Name;
        itemAttributeType.Description = request.Description;
        itemAttributeType.IsActive = true;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
