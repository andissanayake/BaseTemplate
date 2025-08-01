using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Specifications.Queries.GetSpecifications;

public class GetSpecificationsQueryHandler : IRequestHandler<GetSpecificationsQuery, GetSpecificationsResponse>
{
    private readonly IAppDbContext _context;

    public GetSpecificationsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetSpecificationsResponse>> HandleAsync(GetSpecificationsQuery request, CancellationToken cancellationToken)
    {
        // Get all specifications for the tenant
        var allSpecifications = await _context.Specification
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
            if (spec.ParentSpecificationId.HasValue && specificationLookup.ContainsKey(spec.ParentSpecificationId.Value))
            {
                var parent = specificationLookup[spec.ParentSpecificationId.Value];
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

        // Build path from current specification up to root
        while (current != null)
        {
            pathParts.Insert(0, current.Name);
            
            if (current.ParentSpecificationId.HasValue && lookup.ContainsKey(current.ParentSpecificationId.Value))
            {
                current = lookup[current.ParentSpecificationId.Value];
            }
            else
            {
                break;
            }
        }

        return string.Join(" / ", pathParts);
    }
}
