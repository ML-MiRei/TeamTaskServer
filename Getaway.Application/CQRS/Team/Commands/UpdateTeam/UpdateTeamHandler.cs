using Getaway.Application.ServicesInterfaces;
using MediatR;

namespace Getaway.Application.CQRS.Team.Commands.UpdateTeam
{
    public class UpdateTeamHandler(ITeamRepository teamRepository) : IRequestHandler<UpdateTeamCommand>
    {
        public async Task Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
        {
            try
            {
                teamRepository.UpdateTeam(new Core.Entities.TeamEntity() { Name = request.Name, TeamLeadTag = request.LeadTag, ID = request.TeamId });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
