using Getaway.Core.Entities;
using MediatR;

namespace Getaway.Application.CQRS.Team.Queries.GetTeamByTag
{
    public class GetTeamByTagQuery : IRequest<TeamEntity>
    {
        public string TeamTag { get; set; }

    }
}
