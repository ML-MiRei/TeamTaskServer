using Getaway.Core.Entities;
using MediatR;

namespace Getaway.Application.CQRS.Team.Queries.GetTeam
{
    public class GetTeamQuery : IRequest<TeamEntity>
    {
        public int TeamId { get; set; }

    }
}
