using MediatR;

using Getaway.Core.Entities;
using Getaway.Application.ServicesInterfaces;

namespace Getaway.Application.CQRS.Team.Queries.GetTeams
{
    public class GetTeamsHandler(ITeamRepository teamRepository) : IRequestHandler<GetTeamsQuery, List<TeamEntity>>
    {
        public async Task<List<TeamEntity>> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
        {
            return await teamRepository.GetListTeams(request.UserId);
        }
    }
}
