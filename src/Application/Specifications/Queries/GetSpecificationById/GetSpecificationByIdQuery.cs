using BaseTemplate.Application.Common.RequestHandler;

namespace BaseTemplate.Application.Specifications.Queries.GetSpecificationById;

public record GetSpecificationByIdQuery(int Id) : IRequest<GetSpecificationByIdResponse>; 