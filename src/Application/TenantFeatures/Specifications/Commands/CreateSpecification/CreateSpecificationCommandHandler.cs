namespace BaseTemplate.Application.TenantFeatures.Specifications.Commands.CreateSpecification;

public class CreateSpecificationCommandHandler(IAppDbContext context) : IRequestHandler<CreateSpecificationCommand, int>
{
    private readonly IAppDbContext _context = context;
    public async Task<Result<int>> HandleAsync(CreateSpecificationCommand request, CancellationToken cancellationToken)
    {

        var specification = new Specification()
        {
            Name = request.Name,
            Description = request.Description,
            ParentSpecificationId = request.ParentSpecificationId
        };

        _context.Specification.Add(specification);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(specification.Id);
    }
}
