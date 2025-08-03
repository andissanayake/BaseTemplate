namespace BaseTemplate.Application.TenantFeatures.CharacteristicTypes.Commands.CreateCharacteristicType;

public class CreateCharacteristicTypeCommandHandler(IAppDbContext context) : IRequestHandler<CreateCharacteristicTypeCommand, int>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<int>> HandleAsync(CreateCharacteristicTypeCommand request, CancellationToken cancellationToken)
    {

        var itemAttributeType = new CharacteristicType
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
        };

        _context.CharacteristicType.Add(itemAttributeType);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(itemAttributeType.Id);
    }
}
