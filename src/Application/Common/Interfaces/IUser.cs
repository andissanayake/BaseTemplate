namespace BaseTemplate.Application.Common.Interfaces;

public interface IUser
{
    bool? IsAuthenticated { get; }
    string? Identifier { get; }
    string? Name { get; }
    string? Email { get; }
}
