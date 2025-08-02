namespace BaseTemplate.Application.TenantFeatures.Specifications.Commands.DeleteSpecification;

[Authorize(Roles = Roles.SpecificationManager)]
public record DeleteSpecificationCommand(int Id) : IRequest<bool>;
