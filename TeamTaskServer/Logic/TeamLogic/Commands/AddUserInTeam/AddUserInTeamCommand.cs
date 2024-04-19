using MediatR;

namespace ApiGetaway.Logic.TeamLogic.Commands.AddUserInTeam
{
    public class AddUserInTeamCommand : IRequest
    {
        public string UserTag { get; set; }
        public string TeamTag{ get; set; }
    }
}
