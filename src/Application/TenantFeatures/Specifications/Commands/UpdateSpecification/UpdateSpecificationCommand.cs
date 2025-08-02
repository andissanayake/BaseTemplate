namespace BaseTemplate.Application.TenantFeatures.Specifications.Commands.UpdateSpecification;

[Authorize(Roles = Roles.SpecificationManager)]
public record UpdateSpecificationCommand : IRequest<bool>
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int? ParentSpecificationId { get; init; }
}
