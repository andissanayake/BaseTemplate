namespace BaseTemplate.Application.Users.Queries.GetUser;

[Authorize]
public record GetUserQuery : IRequest<GetUserResponse>
{
} 