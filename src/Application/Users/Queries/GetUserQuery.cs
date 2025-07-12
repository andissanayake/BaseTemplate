namespace BaseTemplate.Application.Users.Queries;

[Authorize]
public record GetUserQuery : IRequest<GetUserResponse>
{
} 