using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.ItemAttributeTypes.Commands.DeleteItemAttributeType;

public class DeleteItemAttributeTypeCommandHandler(IAppDbContext context) : IRequestHandler<DeleteItemAttributeTypeCommand, bool>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<bool>> HandleAsync(DeleteItemAttributeTypeCommand request, CancellationToken cancellationToken)
    {
        var itemAttributeType = await _context.ItemAttributeType
            .SingleAsync(t => t.Id == request.Id, cancellationToken);

        itemAttributeType.IsDeleted = true;
        _context.ItemAttributeType.Update(itemAttributeType);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
