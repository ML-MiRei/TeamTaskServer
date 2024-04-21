using MediatR;

using Getaway.Core.Entities;
using Getaway.Application.ServicesInterfaces;
using Getaway.Application.ReturnsModels;

namespace Getaway.Application.CQRS.Team.Queries.GetTeam
{
    public class GetTeamHandler(ITeamRepository teamRepository) : IRequestHandler<GetTeamQuery, TeamEntity>
    {
        public async Task<TeamEntity> Handle(GetTeamQuery request, CancellationToken cancellationToken)
        {
            return await teamRepository.GetTeam(request.TeamId);
        }
    }
}
