using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Items.Commands.UpdateItem;

public class UpdateItemCommandHandler(IAppDbContext context) : IRequestHandler<UpdateItemCommand, bool>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<bool>> HandleAsync(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Item
            .Include(i => i.ItemCharacteristicTypeList)
            .SingleAsync(i => i.Id == request.Id, cancellationToken);

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.Category = request.Category;
        entity.IsActive = request.IsActive;
        entity.SpecificationId = request.SpecificationId;

        // Handle characteristic type relationships
        if (request.CharacteristicTypeIds.Any())
        {
            // Remove existing characteristic type relationships
            var existingCharacteristicTypes = await _context.ItemCharacteristicType
                .Where(ict => ict.ItemId == request.Id)
                .ToListAsync(cancellationToken);
            
            _context.ItemCharacteristicType.RemoveRange(existingCharacteristicTypes);

            // Add new characteristic type relationships
            var newCharacteristicTypes = request.CharacteristicTypeIds.Select(characteristicTypeId => new ItemCharacteristicType
            {
                ItemId = request.Id,
                CharacteristicTypeId = characteristicTypeId
            }).ToList();

            _context.ItemCharacteristicType.AddRange(newCharacteristicTypes);
        }
        else
        {
            // Remove all characteristic type relationships if none provided
            var existingCharacteristicTypes = await _context.ItemCharacteristicType
                .Where(ict => ict.ItemId == request.Id)
                .ToListAsync(cancellationToken);
            
            _context.ItemCharacteristicType.RemoveRange(existingCharacteristicTypes);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
