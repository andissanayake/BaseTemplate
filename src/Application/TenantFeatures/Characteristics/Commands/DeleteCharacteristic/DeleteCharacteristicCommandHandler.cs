using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Characteristics.Commands.DeleteCharacteristic;

public class DeleteCharacteristicCommandHandler(IAppDbContext context) : IRequestHandler<DeleteCharacteristicCommand, bool>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<bool>> HandleAsync(DeleteCharacteristicCommand request, CancellationToken cancellationToken)
    {
        var itemAttribute = await _context.Characteristic
            .SingleAsync(a => a.Id == request.Id, cancellationToken);

        itemAttribute.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
