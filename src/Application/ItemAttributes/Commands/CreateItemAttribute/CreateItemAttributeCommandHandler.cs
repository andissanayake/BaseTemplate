using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.ItemAttributes.Commands.CreateItemAttribute;

public class CreateItemAttributeCommandHandler(IAppDbContext context) : IRequestHandler<CreateItemAttributeCommand, int>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<int>> HandleAsync(CreateItemAttributeCommand request, CancellationToken cancellationToken)
    {

        // Check if code already exists for this tenant
        var existingAttribute = await _context.ItemAttribute
            .FirstOrDefaultAsync(a => a.Code == request.Code, cancellationToken);

        if (existingAttribute != null)
        {
            return Result<int>.Validation("Code must be unique within the tenant",
                                new Dictionary<string, string[]>
                                {
                                    ["Code"] = [$"Code must be unique within the tenant."]
                                });
        }

        var itemAttribute = new ItemAttribute
        {
            Name = request.Name,
            Code = request.Code,
            Value = request.Value,
            ItemAttributeTypeId = request.ItemAttributeTypeId,
            IsActive = true
        };

        _context.ItemAttribute.Add(itemAttribute);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(itemAttribute.Id);
    }
}
