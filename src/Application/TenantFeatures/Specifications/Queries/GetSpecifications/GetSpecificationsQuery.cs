namespace BaseTemplate.Application.TenantFeatures.Specifications.Queries.GetSpecifications;

[Authorize(Roles = Roles.SpecificationManager)]
public record GetSpecificationsQuery : IRequest<GetSpecificationsResponse>;
