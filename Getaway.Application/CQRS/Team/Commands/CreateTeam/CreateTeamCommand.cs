using Getaway.Core.Entities;
using MediatR;

namespace Getaway.Application.CQRS.Team.Commands.CreateTeam
{
    public class CreateTeamCommand : IRequest<TeamEntity>
    {
        public int UserId { get; set; }
        public string Name { get; set; }
    }
}
