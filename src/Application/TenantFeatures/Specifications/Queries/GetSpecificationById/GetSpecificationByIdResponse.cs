namespace BaseTemplate.Application.TenantFeatures.Specifications.Queries.GetSpecificationById;

public record GetSpecificationByIdResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int? ParentSpecificationId { get; init; }
    public string? ParentSpecificationName { get; init; }
} 