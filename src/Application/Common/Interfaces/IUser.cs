namespace BaseTemplate.Application.Common.Interfaces;

public interface IUser
{
    string Identifier { get; }
    string Name { get; }
    string Email { get; }
}
