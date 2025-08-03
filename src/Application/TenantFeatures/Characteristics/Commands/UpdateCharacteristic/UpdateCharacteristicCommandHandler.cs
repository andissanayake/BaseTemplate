using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Characteristics.Commands.UpdateCharacteristic;

public class UpdateCharacteristicCommandHandler : IRequestHandler<UpdateCharacteristicCommand, bool>
{
    private readonly IAppDbContext _context;
    public UpdateCharacteristicCommandHandler(IAppDbContext context)
    {
        _context = context; ;
    }

    public async Task<Result<bool>> HandleAsync(UpdateCharacteristicCommand request, CancellationToken cancellationToken)
    {

        var itemAttribute = await _context.Characteristic
            .SingleAsync(a => a.Id == request.Id, cancellationToken);

        // Check if code already exists for this tenant (excluding current item)
        var existingAttribute = await _context.Characteristic.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Code == request.Code && a.Id != request.Id, cancellationToken);

        if (existingAttribute != null)
        {
            return Result<bool>.Validation("Code must be unique within the tenant",
                                                new Dictionary<string, string[]>
                                                {
                                                    ["Code"] = [$"Code must be unique within the tenant."]
                                                });
        }

        itemAttribute.Name = request.Name;
        itemAttribute.Code = request.Code;
        itemAttribute.Value = request.Value;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
