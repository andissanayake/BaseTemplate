using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.ItemAttributes.Commands.DeleteItemAttribute;

public class DeleteItemAttributeCommandHandler(IAppDbContext context) : IRequestHandler<DeleteItemAttributeCommand, bool>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<bool>> HandleAsync(DeleteItemAttributeCommand request, CancellationToken cancellationToken)
    {
        var itemAttribute = await _context.ItemAttribute
            .SingleAsync(a => a.Id == request.Id, cancellationToken);

        itemAttribute.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
