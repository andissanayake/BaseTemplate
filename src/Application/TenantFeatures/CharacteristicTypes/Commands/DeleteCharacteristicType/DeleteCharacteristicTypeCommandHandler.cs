using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.CharacteristicTypes.Commands.DeleteCharacteristicType;

public class DeleteCharacteristicTypeCommandHandler(IAppDbContext context) : IRequestHandler<DeleteCharacteristicTypeCommand, bool>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<bool>> HandleAsync(DeleteCharacteristicTypeCommand request, CancellationToken cancellationToken)
    {
        var itemAttributeType = await _context.CharacteristicType
            .SingleAsync(t => t.Id == request.Id, cancellationToken);

        itemAttributeType.IsDeleted = true;
        _context.CharacteristicType.Update(itemAttributeType);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
