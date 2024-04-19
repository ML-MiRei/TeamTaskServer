using MediatR;

namespace Getaway.Application.CQRS.Team.Commands.UpdateTeam
{
    public class UpdateTeamCommand : IRequest
    {
        public string? LeadTag { get; set; }
        public int TeamId { get; set; }
        public string? Name { get; set; }
    }
}
