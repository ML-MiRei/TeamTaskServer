using MediatR;

namespace ApiGetaway.Logic.UserLogic.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest
    {
        public int UserId { get; set; }
    }
}
