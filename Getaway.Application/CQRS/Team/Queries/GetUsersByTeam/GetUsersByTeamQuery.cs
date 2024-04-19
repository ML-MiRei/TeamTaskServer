using Getaway.Core.Entities;
using MediatR;

namespace Getaway.Application.CQRS.Team.Queries.GetUsersByTeam
{
    public class GetUsersByTeamQuery : IRequest<List<UserEntity>>
    {
        public int TeamId { get; set; }

    }
}
