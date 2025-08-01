namespace BaseTemplate.Application.Items.Commands.CreateItem;

public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, int>
{
    private readonly IAppDbContext _context;
    public CreateItemCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> HandleAsync(CreateItemCommand request, CancellationToken cancellationToken)
    {

        var entity = new Item
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            IsActive = true
        };

        _context.Item.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(entity.Id);
    }
}
