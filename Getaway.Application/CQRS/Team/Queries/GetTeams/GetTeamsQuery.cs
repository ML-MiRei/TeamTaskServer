using Getaway.Core.Entities;
using MediatR;

namespace Getaway.Application.CQRS.Team.Queries.GetTeams
{
    public class GetTeamsQuery : IRequest<List<TeamEntity>>
    {
        public int UserId { get; set; }

    }
}
