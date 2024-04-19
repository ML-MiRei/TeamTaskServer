using MediatR;

namespace Getaway.Application.CQRS.Team.Commands.AddUserInTeam
{
    public class AddUserInTeamCommand : IRequest
    {
        public string UserTag { get; set; }
        public int TeamId { get; set; }
    }
}
