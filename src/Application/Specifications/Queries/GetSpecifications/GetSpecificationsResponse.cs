namespace BaseTemplate.Application.Specifications.Queries.GetSpecifications;

public record GetSpecificationsResponse
{
    public List<SpecificationBriefDto> Specifications { get; init; } = new();
}

public record SpecificationBriefDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int? ParentSpecificationId { get; init; }
    public string FullPath { get; set; } = string.Empty;
    public List<SpecificationBriefDto> Children { get; init; } = new();
}
