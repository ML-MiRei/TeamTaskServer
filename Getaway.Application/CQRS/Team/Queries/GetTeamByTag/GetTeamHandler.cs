using MediatR;

using Getaway.Core.Entities;
using Getaway.Application.ServicesInterfaces;
using Getaway.Application.ReturnsModels;

namespace Getaway.Application.CQRS.Team.Queries.GetTeamByTag
{
    public class GetTeamByTagHandler(ITeamRepository teamRepository) : IRequestHandler<GetTeamByTagQuery, TeamEntity>
    {
        public async Task<TeamEntity> Handle(GetTeamByTagQuery request, CancellationToken cancellationToken)
        {
            return await teamRepository.GetTeamByTag(request.TeamTag);
        }
    }
}
