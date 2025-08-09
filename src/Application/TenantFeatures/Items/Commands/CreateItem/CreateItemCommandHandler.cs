namespace BaseTemplate.Application.TenantFeatures.Items.Commands.CreateItem;

public class CreateItemCommandHandler(IAppDbContext context) : IRequestHandler<CreateItemCommand, int>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<int>> HandleAsync(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new Item
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            Tags = request.Tags,
            IsActive = true,
            SpecificationId = request.SpecificationId,
            HasVariant = request.HasVariant.HasValue ? request.HasVariant.Value : false,
        };

        _context.Item.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(entity.Id);
    }
}
