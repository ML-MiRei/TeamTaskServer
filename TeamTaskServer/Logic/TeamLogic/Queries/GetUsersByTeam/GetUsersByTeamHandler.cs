using MediatR;
using ApiGetaway.Core.Exceptions;
using ApiGetaway.Core.ConnectionSettings;

namespace ApiGetaway.Logic.TeamLogic.Queries.GetUsersByTeam
{
    public class GetUsersByTeamHandler : IRequestHandler<GetUsersByTeamQuery, List<string>>
    {
        public async Task<List<string>> Handle(GetUsersByTeamQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await Connections.TeamServiceClient.GetUsersAsync(new GetUsersRequest() { TeamTag = request.TeamTag});
                return user.UsersTags.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new NotFoundException();
            }
        }
    }
}
