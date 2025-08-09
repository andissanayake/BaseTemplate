using Microsoft.EntityFrameworkCore;
using BaseTemplate.Application.TenantFeatures.Items.Queries.GetItemVariantByItemId;

namespace BaseTemplate.Application.TenantFeatures.Items.Commands.UpdateItemCharacteristicType;

public class UpdateItemCharacteristicTypeCommandHandler(IAppDbContext context, IMediator mediator) : IRequestHandler<UpdateItemCharacteristicTypeCommand, bool>
{
    private readonly IAppDbContext _context = context;
    private readonly IMediator _mediator = mediator;

    public async Task<Result<bool>> HandleAsync(UpdateItemCharacteristicTypeCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Execute existing characteristic type update flow
        var updateResult = await UpdateCharacteristicTypesAsync(request, cancellationToken);
        if (!ResultCodeMapper.IsSuccess(updateResult.Code))
        {
            return updateResult;
        }

        // Step 2: Handle item variants based on detected scenarios
        await HandleItemVariantScenariosAsync(request.ItemId, cancellationToken);

        return Result<bool>.Success(true);
    }

    private async Task<Result<bool>> UpdateCharacteristicTypesAsync(UpdateItemCharacteristicTypeCommand request, CancellationToken cancellationToken)
    {
        // Verify item exists
        var item = await _context.Item
            .SingleAsync(i => i.Id == request.ItemId, cancellationToken);

        // Verify that all provided characteristic type IDs exist and are active
        if (request.CharacteristicTypeIds.Count != 0)
        {
            var existingCharacteristicTypeIds = await _context.CharacteristicType
                .Where(ct => request.CharacteristicTypeIds.Contains(ct.Id) && ct.IsActive)
                .Select(ct => ct.Id)
                .ToListAsync(cancellationToken);

            var invalidIds = request.CharacteristicTypeIds.Except(existingCharacteristicTypeIds).ToList();
            if (invalidIds.Count != 0)
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
        if (request.CharacteristicTypeIds.Count != 0)
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

    private async Task HandleItemVariantScenariosAsync(int itemId, CancellationToken cancellationToken)
    {
        // Get item with its variants and characteristic types
        var item = await _context.Item
            .Include(i => i.ItemCharacteristicTypeList)
                .ThenInclude(ict => ict.CharacteristicType)
                    .ThenInclude(ct => ct.CharacteristicList)
            .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

        if (item == null || !item.HasVariant)
        {
            return; // No variant handling needed
        }

        var existingVariants = await _context.ItemVariant
            .Include(iv => iv.ItemVariantCharacteristicList)
                .ThenInclude(ivc => ivc.Characteristic)
                    .ThenInclude(c => c.CharacteristicType)
            .Where(iv => iv.ItemId == itemId)
            .ToListAsync(cancellationToken);

        var scenario = await DetectVariantScenarioAsync(item, existingVariants, cancellationToken);

        switch (scenario)
        {
            case VariantScenario.NoVariantsGenerated:
                // Scenario 1: No variants generated yet - generate initial variants
                await GenerateInitialItemVariantsAsync(item, cancellationToken);
                break;

            case VariantScenario.NewCharacteristicsAdded:
                // Scenario 2: Characteristic types unchanged but new characteristics added - generate additional variants
                await GenerateAdditionalVariantsAsync(item, existingVariants, cancellationToken);
                break;

            case VariantScenario.CharacteristicTypesChanged:
                // Scenario 3: Characteristic types changed - regenerate all variants
                await RegenerateItemVariantsAsync(item, existingVariants, cancellationToken);
                break;
        }
    }

    private async Task<VariantScenario> DetectVariantScenarioAsync(Item item, List<ItemVariant> existingVariants, CancellationToken cancellationToken)
    {
        if (!existingVariants.Any())
        {
            // Scenario 1: No existing variants - need to generate initial variants
            return VariantScenario.NoVariantsGenerated;
        }

        // Get all characteristics for current characteristic types
        var currentCharacteristicTypeIds = item.ItemCharacteristicTypeList.Select(ict => ict.CharacteristicTypeId).ToList();
        var allCurrentCharacteristics = await _context.Characteristic
            .Where(c => currentCharacteristicTypeIds.Contains(c.CharacteristicTypeId))
            .ToListAsync(cancellationToken);

        // Get characteristic type IDs from existing variants
        var existingCharacteristicTypeIds = existingVariants
            .SelectMany(v => v.ItemVariantCharacteristicList.Select(ivc => ivc.Characteristic.CharacteristicTypeId))
            .Distinct()
            .ToList();

        // Check if characteristic types have changed
        var currentTypeIdsSet = currentCharacteristicTypeIds.ToHashSet();
        var existingTypeIdsSet = existingCharacteristicTypeIds.ToHashSet();

        if (!currentTypeIdsSet.SetEquals(existingTypeIdsSet))
        {
            // Scenario 3: Characteristic types have changed - regenerate all variants
            return VariantScenario.CharacteristicTypesChanged;
        }

        // Check if new characteristics have been added to existing types
        // This happens when variants were generated initially, but later new characteristics 
        // (e.g., new colors) were added to the characteristic types
        var existingCharacteristicIds = existingVariants
            .SelectMany(v => v.ItemVariantCharacteristicList.Select(ivc => ivc.CharacteristicId))
            .Distinct()
            .ToHashSet();

        var currentCharacteristicIds = allCurrentCharacteristics.Select(c => c.Id).ToHashSet();

        if (!existingCharacteristicIds.IsSupersetOf(currentCharacteristicIds))
        {
            // Scenario 2: New characteristics have been added to existing types
            // (existing variants don't cover all available characteristics)
            return VariantScenario.NewCharacteristicsAdded;
        }

        // All variants are up to date - no action needed
        return VariantScenario.NoVariantsGenerated; // This shouldn't happen but fallback
    }

    private async Task GenerateInitialItemVariantsAsync(Item item, CancellationToken cancellationToken)
    {
        // Generate initial variants based on current characteristic types
        var characteristicsByType = await GetCharacteristicsByTypeAsync(item, cancellationToken);
        
        if (characteristicsByType.Any())
        {
            var variantCombinations = GenerateVariantCombinations(characteristicsByType);
            
            foreach (var combination in variantCombinations)
            {
                // Get characteristic codes for this combination ordered by characteristic type
                var characteristicCodes = await _context.Characteristic
                    .Include(c => c.CharacteristicType)
                    .Where(c => combination.Contains(c.Id))
                    .OrderBy(c => c.CharacteristicType.Name)
                    .ThenBy(c => c.Code)
                    .Select(c => c.Code)
                    .ToListAsync(cancellationToken);
                
                var variantCode = $"{item.Code}-{string.Join("-", characteristicCodes)}";
                
                var newVariant = new ItemVariant
                {
                    ItemId = item.Id,
                    Code = variantCode,
                    Price = 0 // Default price, can be updated later
                };
                
                _context.ItemVariant.Add(newVariant);
                await _context.SaveChangesAsync(cancellationToken); // Save to get variant ID

                // Add variant characteristics
                foreach (var characteristicId in combination)
                {
                    var variantCharacteristic = new ItemVariantCharacteristic
                    {
                        ItemVariantId = newVariant.Id,
                        CharacteristicId = characteristicId
                    };
                    _context.ItemVariantCharacteristic.Add(variantCharacteristic);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task GenerateAdditionalVariantsAsync(Item item, List<ItemVariant> existingVariants, CancellationToken cancellationToken)
    {
        // Get all current characteristics grouped by type
        var characteristicsByType = await GetCharacteristicsByTypeAsync(item, cancellationToken);
        
        // Get existing variant combinations
        var existingCombinations = existingVariants.Select(v => 
            v.ItemVariantCharacteristicList.Select(ivc => ivc.CharacteristicId).OrderBy(id => id).ToList()
        ).ToHashSet(new ListComparer<int>());

        // Generate all possible combinations
        var allPossibleCombinations = GenerateVariantCombinations(characteristicsByType);
        
        // Find new combinations that don't exist yet
        var newCombinations = allPossibleCombinations
            .Where(combination => !existingCombinations.Contains(combination.OrderBy(id => id).ToList()))
            .ToList();

        // Create variants for new combinations
        foreach (var combination in newCombinations)
        {
            // Get characteristic codes for this combination ordered by characteristic type
            var characteristicCodes = await _context.Characteristic
                .Include(c => c.CharacteristicType)
                .Where(c => combination.Contains(c.Id))
                .OrderBy(c => c.CharacteristicType.Name)
                .ThenBy(c => c.Code)
                .Select(c => c.Code)
                .ToListAsync(cancellationToken);
            
            var variantCode = $"{item.Code}-{string.Join("-", characteristicCodes)}";
            
            var newVariant = new ItemVariant
            {
                ItemId = item.Id,
                Code = variantCode,
                Price = 0 // Default price, can be updated later
            };
            
            _context.ItemVariant.Add(newVariant);
            await _context.SaveChangesAsync(cancellationToken); // Save to get variant ID

            // Add variant characteristics
            foreach (var characteristicId in combination)
            {
                var variantCharacteristic = new ItemVariantCharacteristic
                {
                    ItemVariantId = newVariant.Id,
                    CharacteristicId = characteristicId
                };
                _context.ItemVariantCharacteristic.Add(variantCharacteristic);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task RegenerateItemVariantsAsync(Item item, List<ItemVariant> existingVariants, CancellationToken cancellationToken)
    {
        // Remove existing variants and their characteristics
        foreach (var variant in existingVariants)
        {
            _context.ItemVariantCharacteristic.RemoveRange(variant.ItemVariantCharacteristicList);
        }
        _context.ItemVariant.RemoveRange(existingVariants);

        // Generate new variants based on current characteristic types
        var characteristicsByType = await GetCharacteristicsByTypeAsync(item, cancellationToken);
        
        if (characteristicsByType.Any())
        {
            var variantCombinations = GenerateVariantCombinations(characteristicsByType);
            
            foreach (var combination in variantCombinations)
            {
                // Get characteristic codes for this combination ordered by characteristic type
                var characteristicCodes = await _context.Characteristic
                    .Include(c => c.CharacteristicType)
                    .Where(c => combination.Contains(c.Id))
                    .OrderBy(c => c.CharacteristicType.Name)
                    .ThenBy(c => c.Code)
                    .Select(c => c.Code)
                    .ToListAsync(cancellationToken);
                
                var variantCode = $"{item.Code}-{string.Join("-", characteristicCodes)}";
                
                var newVariant = new ItemVariant
                {
                    ItemId = item.Id,
                    Code = variantCode,
                    Price = 0 // Default price, can be updated later
                };
                
                _context.ItemVariant.Add(newVariant);
                await _context.SaveChangesAsync(cancellationToken); // Save to get variant ID

                // Add variant characteristics
                foreach (var characteristicId in combination)
                {
                    var variantCharacteristic = new ItemVariantCharacteristic
                    {
                        ItemVariantId = newVariant.Id,
                        CharacteristicId = characteristicId
                    };
                    _context.ItemVariantCharacteristic.Add(variantCharacteristic);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<Dictionary<int, List<Characteristic>>> GetCharacteristicsByTypeAsync(Item item, CancellationToken cancellationToken)
    {
        var characteristicTypeIds = item.ItemCharacteristicTypeList.Select(ict => ict.CharacteristicTypeId).ToList();

        return await _context.Characteristic
            .Where(c => characteristicTypeIds.Contains(c.CharacteristicTypeId))
            .GroupBy(c => c.CharacteristicTypeId)
            .ToDictionaryAsync(g => g.Key, g => g.ToList(), cancellationToken);
    }

    private static List<List<int>> GenerateVariantCombinations(Dictionary<int, List<Characteristic>> characteristicsByType)
    {
        var allCombinations = new List<List<int>>();
        var characteristicLists = characteristicsByType.Values.ToList();

        if (!characteristicLists.Any())
        {
            return allCombinations;
        }

        GenerateCombinationsRecursive(characteristicLists, 0, new List<int>(), allCombinations);
        return allCombinations;
    }

    private static void GenerateCombinationsRecursive(
        List<List<Characteristic>> characteristicLists,
        int currentTypeIndex,
        List<int> currentCombination,
        List<List<int>> allCombinations)
    {
        if (currentTypeIndex >= characteristicLists.Count)
        {
            allCombinations.Add(new List<int>(currentCombination));
            return;
        }

        foreach (var characteristic in characteristicLists[currentTypeIndex])
        {
            currentCombination.Add(characteristic.Id);
            GenerateCombinationsRecursive(characteristicLists, currentTypeIndex + 1, currentCombination, allCombinations);
            currentCombination.RemoveAt(currentCombination.Count - 1);
        }
    }

    /// <summary>
    /// Gets all item variants for the specified item ID using the query handler
    /// </summary>
    /// <param name="itemId">The item ID to get variants for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of item variant DTOs</returns>
    public async Task<Result<List<ItemVariantDto>>> GetItemVariantsAsync(int itemId, CancellationToken cancellationToken)
    {
        var query = new GetItemVariantsByItemIdQuery(itemId);
        return await _mediator.SendAsync(query, cancellationToken);
    }
}

public enum VariantScenario
{
    NoVariantsGenerated,       // Scenario 1: No variants generated yet
    NewCharacteristicsAdded,   // Scenario 2: New characteristics added to existing types
    CharacteristicTypesChanged // Scenario 3: Characteristic types changed
}

public class ListComparer<T> : IEqualityComparer<List<T>> where T : IComparable<T>
{
    public bool Equals(List<T>? x, List<T>? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        return x.SequenceEqual(y);
    }

    public int GetHashCode(List<T> obj)
    {
        if (obj == null) return 0;
        return obj.Aggregate(0, (hash, item) => hash ^ item.GetHashCode());
    }
}
