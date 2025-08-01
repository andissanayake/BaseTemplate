using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Specifications.Queries.GetSpecificationById;

public class GetSpecificationByIdQueryHandler : IRequestHandler<GetSpecificationByIdQuery, GetSpecificationByIdResponse>
{
    private readonly IAppDbContext _context;

    public GetSpecificationByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetSpecificationByIdResponse>> HandleAsync(GetSpecificationByIdQuery request, CancellationToken cancellationToken)
    {
        var specification = await _context.Specification
            .Include(s => s.ParentSpecification)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (specification == null)
            return Result<GetSpecificationByIdResponse>.NotFound($"Specification with ID {request.Id} not found.");

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
