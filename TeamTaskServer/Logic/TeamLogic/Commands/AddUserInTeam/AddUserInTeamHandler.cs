using ApiGetaway.Core.ConnectionSettings;
using MediatR;

namespace ApiGetaway.Logic.TeamLogic.Commands.AddUserInTeam
{
    public class UpdateTeamHandler : IRequestHandler<AddUserInTeamCommand>
    {
        public async Task Handle(AddUserInTeamCommand request, CancellationToken cancellationToken)
        {
            try
            {
               await Connections.TeamServiceClient.AddUserInTeamAsync(new AddUserTeamRequest() { TeamTag = request.TeamTag, UserTag = request.UserTag });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
