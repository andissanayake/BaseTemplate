namespace BaseTemplate.Application.TenantFeatures.Specifications.Queries.GetSpecificationById;

[Authorize(Roles = Roles.SpecificationManager)]
public record GetSpecificationByIdQuery(int Id) : IRequest<GetSpecificationByIdResponse>;
