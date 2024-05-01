using MediatR;
using Getaway.Core.Entities;
using Getaway.Application.ServicesInterfaces;
using Getaway.Application.RepositoriesInterfaces;

namespace Getaway.Application.CQRS.Team.Queries.GetUsersByChhat
{
    public class GetUsersByChatHandler(IMessangerRepository messangerRepository) : IRequestHandler<GetUsersByChatQuery, List<UserEntity>>
    {
        public async Task<List<UserEntity>> Handle(GetUsersByChatQuery request, CancellationToken cancellationToken)
        {

            return await messangerRepository.GetUsersInChat(request.ChatId);

        }
    }
}
