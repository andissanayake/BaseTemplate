using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.ItemAttributes.Commands.DeleteItemAttribute;

public class DeleteItemAttributeCommandHandler : IRequestHandler<DeleteItemAttributeCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeleteItemAttributeCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> HandleAsync(DeleteItemAttributeCommand request, CancellationToken cancellationToken)
    {
        var itemAttribute = await _context.ItemAttribute
            .SingleAsync(a => a.Id == request.Id, cancellationToken);

        itemAttribute.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
