using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.ItemAttributeTypes.Commands.DeleteItemAttributeType;

public class DeleteItemAttributeTypeCommandHandler : IRequestHandler<DeleteItemAttributeTypeCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeleteItemAttributeTypeCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> HandleAsync(DeleteItemAttributeTypeCommand request, CancellationToken cancellationToken)
    {
        var itemAttributeType = await _context.ItemAttributeType
            .SingleAsync(t => t.Id == request.Id, cancellationToken);

        itemAttributeType.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
