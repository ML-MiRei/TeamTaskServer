using MediatR;

namespace ApiGetaway.Logic.TeamLogic.Queries.GetTeams
{
    public class GetTeamsQuery : IRequest<List<TeamModel>>
    {
        public string UserTag { get; set; }

    }
}
