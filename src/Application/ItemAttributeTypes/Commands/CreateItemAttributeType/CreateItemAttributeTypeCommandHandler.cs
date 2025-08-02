namespace BaseTemplate.Application.ItemAttributeTypes.Commands.CreateItemAttributeType;

public class CreateItemAttributeTypeCommandHandler(IAppDbContext context) : IRequestHandler<CreateItemAttributeTypeCommand, int>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<int>> HandleAsync(CreateItemAttributeTypeCommand request, CancellationToken cancellationToken)
    {

        var itemAttributeType = new ItemAttributeType
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
        };

        _context.ItemAttributeType.Add(itemAttributeType);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(itemAttributeType.Id);
    }
}
