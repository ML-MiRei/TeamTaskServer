using MediatR;

namespace ApiGetaway.Logic.TeamLogic.Queries.GetUsersByTeam
{
    public class GetUsersByTeamQuery : IRequest<List<string>>
    {
        public string TeamTag { get; set; }

    }
}
