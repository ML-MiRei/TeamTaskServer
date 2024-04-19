using MediatR;

namespace ApiGetaway.Logic.TeamLogic.Commands.UpdateTeam
{
    public class UpdateTeamCommand : IRequest
    {
        public string? LeadTag { get; set; }
        public string TeamTag{ get; set; }
        public string? Name { get; set; }
    }
}
