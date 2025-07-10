using System.ComponentModel.DataAnnotations;
using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.Items.Commands.UpdateItem;

[Authorize(Roles = Roles.ItemManager + "," + Roles.TenantOwner)]
public record UpdateItemCommand(int TenantId) : BaseTenantRequest<bool>(TenantId)
{
    public int Id { get; init; }

    [Required]
    [MaxLength(200, ErrorMessage = "The name cannot exceed 200 characters.")]
    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.")]
    public decimal Price { get; init; }

    public string? Category { get; init; }
    public bool IsActive { get; init; }
}

public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;

    public UpdateItemCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<bool>> HandleAsync(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var entity = await uow.QueryFirstOrDefaultAsync<Item>("select * from item where Id = @Id and tenant_id = @TenantId", new { request.Id, request.TenantId });

        if (entity is null)
        {
            return Result<bool>.NotFound($"Item with id {request.Id} not found.");
        }

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.Category = request.Category;
        entity.IsActive = request.IsActive;

        await uow.UpdateAsync(entity);
        return Result<bool>.Success(true);
    }
}
