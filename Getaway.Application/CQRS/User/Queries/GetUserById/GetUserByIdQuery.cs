using Getaway.Core.Entities;
using MediatR;

namespace Getaway.Application.CQRS.User.Queries.GetUserById
{
    public class GetUserByIdQuery : IRequest<UserEntity>
    {
        public int UserId { get; set; }

    }
}
