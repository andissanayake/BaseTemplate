using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Specifications.Queries.GetSpecificationById;

public class GetSpecificationByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetSpecificationByIdQuery, GetSpecificationByIdResponse>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<GetSpecificationByIdResponse>> HandleAsync(GetSpecificationByIdQuery request, CancellationToken cancellationToken)
    {
        var specification = await _context.Specification.AsNoTracking()
            .Include(s => s.ParentSpecification)
            .SingleAsync(s => s.Id == request.Id, cancellationToken);

        var response = new GetSpecificationByIdResponse
        {
            Id = specification.Id,
            Name = specification.Name,
            Description = specification.Description,
            ParentSpecificationId = specification.ParentSpecificationId,
            ParentSpecificationName = specification.ParentSpecification?.Name
        };
        return Result<GetSpecificationByIdResponse>.Success(response);
    }
}
