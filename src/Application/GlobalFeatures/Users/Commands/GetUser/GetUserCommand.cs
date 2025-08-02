namespace BaseTemplate.Application.GlobalFeatures.Users.Commands.GetUser;

[Authorize]
public record GetUserCommand : IRequest<GetUserResponse>
{
}
