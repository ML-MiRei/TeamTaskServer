using Getaway.Application.ServicesInterfaces;
using MediatR;

namespace Getaway.Application.CQRS.Team.Commands.DeleteUserFromTeam
{
    public class DeleteUserFromTeamHandler(ITeamRepository teamRepository) : IRequestHandler<DeleteUserFromTeamCommand>
    {
        public async Task Handle(DeleteUserFromTeamCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await teamRepository.DeleteUserFromTeam(request.UserTag, request.TeamId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
