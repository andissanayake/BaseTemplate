using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Characteristics.Commands.CreateCharacteristic;

public class CreateCharacteristicCommandHandler(IAppDbContext context) : IRequestHandler<CreateCharacteristicCommand, int>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<int>> HandleAsync(CreateCharacteristicCommand request, CancellationToken cancellationToken)
    {

        // Check if code already exists for this tenant
        var existingAttribute = await _context.Characteristic.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Code == request.Code, cancellationToken);

        if (existingAttribute != null)
        {
            return Result<int>.Validation("Code must be unique within the tenant",
                                new Dictionary<string, string[]>
                                {
                                    ["Code"] = [$"Code must be unique within the tenant."]
                                });
        }

        var itemAttribute = new Characteristic
        {
            Name = request.Name,
            Code = request.Code,
            Value = request.Value,
            CharacteristicTypeId = request.ItemAttributeTypeId,
            IsActive = true
        };

        _context.Characteristic.Add(itemAttribute);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(itemAttribute.Id);
    }
}
