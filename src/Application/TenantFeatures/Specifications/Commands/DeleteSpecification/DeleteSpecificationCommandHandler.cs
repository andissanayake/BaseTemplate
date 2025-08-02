using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Specifications.Commands.DeleteSpecification;

public class DeleteSpecificationCommandHandler(IAppDbContext context) : IRequestHandler<DeleteSpecificationCommand, bool>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<bool>> HandleAsync(DeleteSpecificationCommand request, CancellationToken cancellationToken)
    {
        var specification = await _context.Specification
            .SingleAsync(s => s.Id == request.Id, cancellationToken);

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
