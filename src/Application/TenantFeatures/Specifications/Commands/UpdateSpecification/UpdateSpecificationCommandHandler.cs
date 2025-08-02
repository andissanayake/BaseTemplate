using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Specifications.Commands.UpdateSpecification;

public class UpdateSpecificationCommandHandler(IAppDbContext context) : IRequestHandler<UpdateSpecificationCommand, bool>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<bool>> HandleAsync(UpdateSpecificationCommand request, CancellationToken cancellationToken)
    {
        var specification = await _context.Specification
            .SingleAsync(s => s.Id == request.Id, cancellationToken);

        specification.Name = request.Name;
        specification.Description = request.Description;
        specification.ParentSpecificationId = request.ParentSpecificationId;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
