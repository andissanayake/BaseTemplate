using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Items.Commands.DeleteItem;

public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeleteItemCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> HandleAsync(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Item
            .SingleAsync(i => i.Id == request.Id, cancellationToken);
        entity.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
