namespace BaseTemplate.Application.Common.Interfaces;

public interface IUser
{
    bool? IsAuthenticated { get; }
    string? Id { get; }
    string? UserName { get; }
}
