using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Items.Commands.GenerateVariants;

public class GenerateVariantsCommandHandler(IAppDbContext context) : IRequestHandler<GenerateVariantsCommand, bool>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<bool>> HandleAsync(GenerateVariantsCommand request, CancellationToken cancellationToken)
    {
        // Verify item exists and has characteristic types associated
        var item = await _context.Item
            .Include(i => i.ItemCharacteristicTypeList)
            .SingleAsync(i => i.Id == request.ItemId, cancellationToken);

        // Verify that the provided characteristic type IDs are associated with the item
        var itemCharacteristicTypeIds = item.ItemCharacteristicTypeList.Select(ict => ict.CharacteristicTypeId).ToList();
        var invalidCharacteristicTypeIds = request.CharacteristicTypeIds.Except(itemCharacteristicTypeIds).ToList();

        if (invalidCharacteristicTypeIds.Any())
        {
            return Result<bool>.Failure($"Characteristic type IDs {string.Join(", ", invalidCharacteristicTypeIds)} are not associated with this item.");
        }

        // Get characteristics grouped by their types (only for the specified characteristic types)
        var characteristics = await _context.Characteristic
            .Where(c => request.CharacteristicTypeIds.Contains(c.CharacteristicTypeId) && c.IsActive)
            .GroupBy(c => c.CharacteristicTypeId)
            .ToListAsync(cancellationToken);

        if (!characteristics.Any())
        {
            return Result<bool>.Failure("No active characteristics found for the provided characteristic type IDs.");
        }

        // Ensure we have characteristics for all requested characteristic types
        var foundCharacteristicTypeIds = characteristics.Select(g => g.Key).ToList();
        var missingCharacteristicTypeIds = request.CharacteristicTypeIds.Except(foundCharacteristicTypeIds).ToList();

        if (missingCharacteristicTypeIds.Any())
        {
            return Result<bool>.Failure($"No active characteristics found for characteristic type IDs: {string.Join(", ", missingCharacteristicTypeIds)}");
        }

        // Remove existing variants for this item
        var existingVariants = await _context.ItemVariant
            .Include(iv => iv.ItemVariantCharacteristicList)
            .Where(iv => iv.ItemId == request.ItemId)
            .ToListAsync(cancellationToken);

        foreach (var variant in existingVariants)
        {
            _context.ItemVariantCharacteristic.RemoveRange(variant.ItemVariantCharacteristicList);
        }
        _context.ItemVariant.RemoveRange(existingVariants);

        // Generate all possible combinations
        var combinations = GenerateCharacteristicCombinations(characteristics.ToList());
        
        var newVariants = new List<ItemVariant>();
        var variantNumber = 1;

        foreach (var combination in combinations)
        {
            // Generate variant code based on characteristic codes
            var codes = combination.Select(c => c.Code).OrderBy(c => c);
            var variantCode = $"{item.Name}-{string.Join("-", codes)}";

            var variant = new ItemVariant
            {
                ItemId = request.ItemId,
                Code = variantCode,
                Price = 0, // Default price, can be updated later
                ItemVariantCharacteristicList = combination.Select(c => new ItemVariantCharacteristic
                {
                    CharacteristicId = c.Id
                }).ToList()
            };

            newVariants.Add(variant);
            variantNumber++;
        }

        _context.ItemVariant.AddRange(newVariants);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    private static List<List<Characteristic>> GenerateCharacteristicCombinations(List<IGrouping<int, Characteristic>> characteristicGroups)
    {
        var combinations = new List<List<Characteristic>>();
        
        if (!characteristicGroups.Any())
            return combinations;

        // Start with the first group
        var firstGroup = characteristicGroups[0];
        foreach (var characteristic in firstGroup)
        {
            combinations.Add(new List<Characteristic> { characteristic });
        }

        // For each subsequent group, create combinations with existing combinations
        for (int i = 1; i < characteristicGroups.Count; i++)
        {
            var currentGroup = characteristicGroups[i];
            var newCombinations = new List<List<Characteristic>>();

            foreach (var existingCombination in combinations)
            {
                foreach (var characteristic in currentGroup)
                {
                    var newCombination = new List<Characteristic>(existingCombination) { characteristic };
                    newCombinations.Add(newCombination);
                }
            }

            combinations = newCombinations;
        }

        return combinations;
    }
}
