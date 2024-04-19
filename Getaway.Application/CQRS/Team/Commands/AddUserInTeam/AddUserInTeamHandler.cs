
using Getaway.Application.ServicesInterfaces;
using MediatR;

namespace Getaway.Application.CQRS.Team.Commands.AddUserInTeam
{
    public class AddUserInTeamHandler(ITeamRepository teamRepository) : IRequestHandler<AddUserInTeamCommand>
    {
        public async Task Handle(AddUserInTeamCommand request, CancellationToken cancellationToken)
        {
            try
            {
                 teamRepository.AddUserInTeam(teamId: request.TeamId, userTag: request.UserTag);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
