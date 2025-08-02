using BaseTemplate.Application.Common.RequestHandler;

namespace BaseTemplate.Application.TenantFeatures.Specifications.Commands.UpdateSpecification;

public record UpdateSpecificationCommand : IRequest<bool>
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int? ParentSpecificationId { get; init; }
} 