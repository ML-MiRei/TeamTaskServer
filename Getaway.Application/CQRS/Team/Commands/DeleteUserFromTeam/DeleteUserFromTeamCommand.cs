using MediatR;

namespace Getaway.Application.CQRS.Team.Commands.DeleteUserFromTeam
{
    public class DeleteUserFromTeamCommand : IRequest
    {
        public int TeamId { get; set; }
        public string UserTag { get; set; }
    }
}
