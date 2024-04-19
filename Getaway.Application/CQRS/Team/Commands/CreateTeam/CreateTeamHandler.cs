using Getaway.Application.ServicesInterfaces;
using Getaway.Core.Entities;
using MediatR;

namespace Getaway.Application.CQRS.Team.Commands.CreateTeam
{
    public class CreateTeamHandler(ITeamRepository teamRepository) : IRequestHandler<CreateTeamCommand, TeamEntity>
    {
        public async Task<TeamEntity> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await teamRepository.CreateTeam(request.UserId, request.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
