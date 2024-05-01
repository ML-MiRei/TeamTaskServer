using Getaway.Core.Entities;
using MediatR;

namespace Getaway.Application.CQRS.Team.Queries.GetUsersByChhat
{
    public class GetUsersByChatQuery : IRequest<List<UserEntity>>
    {
        public int ChatId { get; set; }

    }
}
