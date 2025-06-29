using System.ComponentModel.DataAnnotations;

namespace BaseTemplate.Application.Items.Commands.CreateItem;

[Authorize]
public record CreateItemCommand : BaseTenantRequest<int>
{

    [Required]
    [MaxLength(200, ErrorMessage = "The name cannot exceed 200 characters.")]
    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.")]
    public decimal Price { get; init; }
    public string? Category { get; init; }
}

public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, int>
{
    private readonly IUnitOfWorkFactory _factory;

    public CreateItemCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<int>> HandleAsync(CreateItemCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();

        // Verify tenant exists and user has access
        var tenant = await uow.GetAsync<Tenant>(request.TenantId);
        if (tenant == null)
        {
            return Result<int>.NotFound($"Tenant with id {request.TenantId} not found.");
        }

        var entity = new Item
        {
            TenantId = request.TenantId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            IsActive = true
        };

        await uow.InsertAsync(entity);
        return Result<int>.Success(entity.Id);
    }
}
