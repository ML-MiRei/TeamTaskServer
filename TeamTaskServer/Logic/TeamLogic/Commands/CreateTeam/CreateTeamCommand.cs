using MediatR;
using ModelsLibrary.Entities;

namespace ApiGetaway.Logic.TeamLogic.Commands.CreateTeam
{
    public class CreateTeamCommand : IRequest<Team>
    {
        public string UserTag { get; set; }
        public string Name{ get; set; }
    }
}
