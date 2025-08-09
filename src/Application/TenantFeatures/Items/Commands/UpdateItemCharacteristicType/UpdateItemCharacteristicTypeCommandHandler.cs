using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Items.Commands.UpdateItemCharacteristicType;

public class UpdateItemCharacteristicTypeCommandHandler(IAppDbContext context) : IRequestHandler<UpdateItemCharacteristicTypeCommand, bool>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<bool>> HandleAsync(UpdateItemCharacteristicTypeCommand request, CancellationToken cancellationToken)
    {
        // Verify item exists
        var item = await _context.Item
            .SingleAsync(i => i.Id == request.ItemId, cancellationToken);

        // Verify that all provided characteristic type IDs exist and are active
        if (request.CharacteristicTypeIds.Any())
        {
            var existingCharacteristicTypeIds = await _context.CharacteristicType
                .Where(ct => request.CharacteristicTypeIds.Contains(ct.Id) && ct.IsActive)
                .Select(ct => ct.Id)
                .ToListAsync(cancellationToken);

            var invalidIds = request.CharacteristicTypeIds.Except(existingCharacteristicTypeIds).ToList();
            if (invalidIds.Any())
            {
                return Result<bool>.Validation($"Invalid or inactive characteristic type IDs: {string.Join(", ", invalidIds)}");
            }
        }

        // Remove existing characteristic type relationships
        var existingCharacteristicTypes = await _context.ItemCharacteristicType
            .Where(ict => ict.ItemId == request.ItemId)
            .ToListAsync(cancellationToken);

        _context.ItemCharacteristicType.RemoveRange(existingCharacteristicTypes);

        // Add new characteristic type relationships
        if (request.CharacteristicTypeIds.Any())
        {
            var newCharacteristicTypes = request.CharacteristicTypeIds.Select(characteristicTypeId => new ItemCharacteristicType
            {
                ItemId = request.ItemId,
                CharacteristicTypeId = characteristicTypeId
            }).ToList();

            _context.ItemCharacteristicType.AddRange(newCharacteristicTypes);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
