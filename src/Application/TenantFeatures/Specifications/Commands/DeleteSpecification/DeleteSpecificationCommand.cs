using BaseTemplate.Application.Common.RequestHandler;

namespace BaseTemplate.Application.TenantFeatures.Specifications.Commands.DeleteSpecification;

public record DeleteSpecificationCommand(int Id) : IRequest<bool>; 