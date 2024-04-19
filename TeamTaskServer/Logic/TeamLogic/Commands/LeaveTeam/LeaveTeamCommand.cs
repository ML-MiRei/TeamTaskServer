using MediatR;

namespace ApiGetaway.Logic.TeamLogic.Commands.LeaveTeam
{
    public class LeaveTeamCommand : IRequest
    {
        public string TeamTag {  get; set; }
        public string UserTag { get; set; }
    }
}
