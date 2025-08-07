using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.CharacteristicTypes.Commands.UpdateCharacteristicType;

public class UpdateCharacteristicTypeCommandHandler(IAppDbContext context) : IRequestHandler<UpdateCharacteristicTypeCommand, bool>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<bool>> HandleAsync(UpdateCharacteristicTypeCommand request, CancellationToken cancellationToken)
    {

        var characteristicType = await _context.CharacteristicType
            .SingleAsync(t => t.Id == request.Id, cancellationToken);

        characteristicType.Name = request.Name;
        characteristicType.Description = request.Description;
        characteristicType.IsActive = true;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
