using MediatR;

namespace Getaway.Application.CQRS.User.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest
    {
        public int UserId { get; set; }
    }
}
