namespace BaseTemplate.Application.TenantFeatures.Specifications.Commands.CreateSpecification;

[Authorize(Roles = Roles.SpecificationManager)]
public record CreateSpecificationCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int? ParentSpecificationId { get; init; }
}
