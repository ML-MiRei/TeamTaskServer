using ApiGetaway.Core.ConnectionSettings;
using MediatR;

namespace ApiGetaway.Logic.TeamLogic.Commands.UpdateTeam
{
    public class UpdateTeamHandler : IRequestHandler<UpdateTeamCommand>
    {
        public async Task Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
        {
            try
            {
               await Connections.TeamServiceClient.UpdateTeamAsync(
                   new UpdateTeamsRequest() { TeamTag = request.TeamTag, TagLead = request.LeadTag, Name = request.Name });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
