using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Specifications.Queries.GetSpecificationById;

public class GetSpecificationByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetSpecificationByIdQuery, GetSpecificationByIdResponse>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<GetSpecificationByIdResponse>> HandleAsync(GetSpecificationByIdQuery request, CancellationToken cancellationToken)
    {
        // Load all specifications to build the complete hierarchy
        var allSpecifications = await _context.Specification.AsNoTracking()
            .ToListAsync(cancellationToken);

        // Create a lookup for quick access
        var specificationLookup = allSpecifications.ToDictionary(s => s.Id);

        // Build the parent hierarchy by setting ParentSpecification references
        foreach (var spec in allSpecifications)
        {
            if (spec.ParentSpecificationId.HasValue && specificationLookup.TryGetValue(spec.ParentSpecificationId.Value, out var parent))
            {
                spec.ParentSpecification = parent;
            }
        }

        // Find the requested specification
        var specification = allSpecifications.Single(s => s.Id == request.Id);

        var response = new GetSpecificationByIdResponse
        {
            Id = specification.Id,
            Name = specification.Name,
            Description = specification.Description,
            ParentSpecificationId = specification.ParentSpecificationId,
            ParentSpecificationName = specification.ParentSpecification?.Name,
            FullPath = specification.FullPath
        };
        return Result<GetSpecificationByIdResponse>.Success(response);
    }
}
