using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Specifications.Queries.GetSpecifications;

public class GetSpecificationsQueryHandler(IAppDbContext context) : IRequestHandler<GetSpecificationsQuery, GetSpecificationsResponse>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<GetSpecificationsResponse>> HandleAsync(GetSpecificationsQuery request, CancellationToken cancellationToken)
    {
        var allSpecifications = await _context.Specification.AsNoTracking()
            .Include(s => s.ParentSpecification)
            .ToListAsync(cancellationToken);

        // Create DTOs with full paths using entity property
        var specificationDtos = allSpecifications.Select(s => new SpecificationBriefDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            ParentSpecificationId = s.ParentSpecificationId,
            FullPath = s.FullPath,
            Children = []
        }).ToList();

        // Create a lookup for quick access
        var specificationLookup = specificationDtos.ToDictionary(s => s.Id);

        // Get only root specifications (those without parents)
        var rootSpecifications = specificationDtos.Where(s => !s.ParentSpecificationId.HasValue).ToList();

        // Build the hierarchical structure
        foreach (var spec in specificationDtos)
        {
            if (spec.ParentSpecificationId.HasValue && specificationLookup.TryGetValue(spec.ParentSpecificationId.Value, out SpecificationBriefDto? value))
            {
                var parent = value;
                parent.Children.Add(spec);
            }
        }

        var response = new GetSpecificationsResponse
        {
            Specifications = rootSpecifications
        };

        return Result<GetSpecificationsResponse>.Success(response);
    }
}
