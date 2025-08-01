using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Specifications.Commands.DeleteSpecification;

public class DeleteSpecificationCommandHandler : IRequestHandler<DeleteSpecificationCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeleteSpecificationCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> HandleAsync(DeleteSpecificationCommand request, CancellationToken cancellationToken)
    {
        var specification = await _context.Specification
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (specification == null)
            return Result<bool>.NotFound($"Specification with ID {request.Id} not found.");

        // Check if there are any child specifications
        var hasChildren = await _context.Specification
            .AnyAsync(s => s.ParentSpecificationId == request.Id, cancellationToken);

        if (hasChildren)
            return Result<bool>.Validation("Cannot delete specification that has child specifications.");

        _context.Specification.Remove(specification);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
