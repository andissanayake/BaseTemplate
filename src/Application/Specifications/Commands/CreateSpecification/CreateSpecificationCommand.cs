using BaseTemplate.Application.Common.RequestHandler;

namespace BaseTemplate.Application.Specifications.Commands.CreateSpecification;

public record CreateSpecificationCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int? ParentSpecificationId { get; init; }
} 