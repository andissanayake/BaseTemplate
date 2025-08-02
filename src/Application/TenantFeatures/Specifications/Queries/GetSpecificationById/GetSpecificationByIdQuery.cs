using BaseTemplate.Application.Common.RequestHandler;

namespace BaseTemplate.Application.TenantFeatures.Specifications.Queries.GetSpecificationById;

public record GetSpecificationByIdQuery(int Id) : IRequest<GetSpecificationByIdResponse>; 