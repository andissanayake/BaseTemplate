namespace BaseTemplate.Application.Items.Queries.GetItemById;

[Authorize]
public record GetItemByIdQuery(int Id) : IRequest<ItemDto>;

public class ItemDto
{
    public int Id { get; init; }
    public int TenantId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public bool IsActive { get; init; }
    public string? Category { get; init; }
}

public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, ItemDto>
{
    private readonly IUnitOfWorkFactory _factory;

    public GetItemByIdQueryHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<ItemDto>> HandleAsync(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var entity = await uow.GetAsync<Item>(request.Id);
        
        if (entity is null)
        {
            return Result<ItemDto>.NotFound($"Item with id {request.Id} not found.");
        }

        var itemDto = new ItemDto
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            IsActive = entity.IsActive,
            Category = entity.Category
        };

        return Result<ItemDto>.Success(itemDto);
    }
} 