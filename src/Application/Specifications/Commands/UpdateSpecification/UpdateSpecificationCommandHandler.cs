using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Specifications.Commands.UpdateSpecification;

public class UpdateSpecificationCommandHandler : IRequestHandler<UpdateSpecificationCommand, bool>
{
    private readonly IAppDbContext _context;

    public UpdateSpecificationCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> HandleAsync(UpdateSpecificationCommand request, CancellationToken cancellationToken)
    {
        var specification = await _context.Specification
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (specification == null)
            return Result<bool>.NotFound($"Specification with ID {request.Id} not found.");

        specification.Name = request.Name;
        specification.Description = request.Description;
        specification.ParentSpecificationId = request.ParentSpecificationId;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
