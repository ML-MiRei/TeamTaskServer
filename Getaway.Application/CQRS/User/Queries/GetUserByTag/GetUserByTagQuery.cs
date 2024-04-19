using Getaway.Core.Entities;
using MediatR;

namespace Getaway.Application.CQRS.User.Queries.GetUserByTag
{
    public class GetUserByTagQuery : IRequest<UserEntity>
    {
        public string UserTag { get; set; }
    }
}
