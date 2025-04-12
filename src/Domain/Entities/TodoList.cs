
namespace BaseTemplate.Domain.Entities;

public class TodoList : BaseAuditableEntity
{
    public string? Title { get; set; }
    public string Colour { get; set; } = ValueObjects.Colour.White.Code;
}
