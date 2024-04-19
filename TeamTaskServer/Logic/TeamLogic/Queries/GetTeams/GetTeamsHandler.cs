using MediatR;
using ApiGetaway.Core.Exceptions;
using ApiGetaway.Core.ConnectionSettings;

namespace ApiGetaway.Logic.TeamLogic.Queries.GetTeams
{
    public class GetTeamsHandler : IRequestHandler<GetTeamsQuery, List<TeamModel>>
    {
        public async Task<List<TeamModel>> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await Connections.TeamServiceClient.GetListTeamsAsync(new GetListTeamsRequest() { UserTag = request.UserTag});
                return user.Teams.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new NotFoundException();
            }
        }
    }
}
