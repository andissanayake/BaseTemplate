using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Specifications.Queries.GetSpecifications;

public class GetSpecificationsQueryHandler(IAppDbContext context) : IRequestHandler<GetSpecificationsQuery, GetSpecificationsResponse>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<GetSpecificationsResponse>> HandleAsync(GetSpecificationsQuery request, CancellationToken cancellationToken)
    {
        var allSpecifications = await _context.Specification.AsNoTracking()
            .Select(s => new SpecificationBriefDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                ParentSpecificationId = s.ParentSpecificationId,
                FullPath = string.Empty, // Will be populated below
                Children = new List<SpecificationBriefDto>()
            })
            .ToListAsync(cancellationToken);

        // Create a lookup for quick access
        var specificationLookup = allSpecifications.ToDictionary(s => s.Id);

        // Populate FullPath for all specifications
        foreach (var spec in allSpecifications)
        {
            spec.FullPath = BuildFullPath(spec, specificationLookup);
        }

        // Get only root specifications (those without parents)
        var rootSpecifications = allSpecifications.Where(s => !s.ParentSpecificationId.HasValue).ToList();

        // Build the hierarchical structure
        foreach (var spec in allSpecifications)
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

    private static string BuildFullPath(SpecificationBriefDto spec, Dictionary<int, SpecificationBriefDto> lookup)
    {
        var pathParts = new List<string>();
        var current = spec;

        while (current != null)
        {
            pathParts.Insert(0, current.Name);

            if (current.ParentSpecificationId.HasValue && lookup.TryGetValue(current.ParentSpecificationId.Value, out SpecificationBriefDto? value))
            {
                current = value;
            }
            else
            {
                break;
            }
        }
        return string.Join(" / ", pathParts);
    }
}
