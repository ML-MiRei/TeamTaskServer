using MediatR;
using Getaway.Core.Entities;
using Getaway.Application.ServicesInterfaces;

namespace Getaway.Application.CQRS.Team.Queries.GetUsersByTeam
{
    public class GetUsersByTeamHandler(ITeamRepository teamRepository) : IRequestHandler<GetUsersByTeamQuery, List<UserEntity>>
    {
        public async Task<List<UserEntity>> Handle(GetUsersByTeamQuery request, CancellationToken cancellationToken)
        {
            //try
            //{
            //    var user = await Connections.TeamServiceClient.GetUsersAsync(new GetUsersRequest() { TeamTag = request.TeamTag });
            //    return user.UsersTags.ToList();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    throw new NotFoundException();
            //}

            return await teamRepository.GetUsers(request.TeamId);

        }
    }
}
