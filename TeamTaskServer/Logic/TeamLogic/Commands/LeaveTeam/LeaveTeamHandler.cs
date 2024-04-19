using ApiGetaway.Core.ConnectionSettings;
using MediatR;

namespace ApiGetaway.Logic.TeamLogic.Commands.LeaveTeam
{
    public class LeaveTeamHandler : IRequestHandler<LeaveTeamCommand>
    {
        public async Task Handle(LeaveTeamCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await Connections.TeamServiceClient.LeaveTeamAsync(new LeaveTeamRequst() { TeamTag = request.TeamTag, UserTag = request.UserTag });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
