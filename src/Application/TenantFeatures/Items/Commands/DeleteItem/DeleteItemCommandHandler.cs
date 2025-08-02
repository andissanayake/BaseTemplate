using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Items.Commands.DeleteItem;

public class DeleteItemCommandHandler(IAppDbContext context) : IRequestHandler<DeleteItemCommand, bool>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<bool>> HandleAsync(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Item
            .SingleAsync(i => i.Id == request.Id, cancellationToken);
        entity.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
