
using Getaway.Application.ServicesInterfaces;
using MediatR;

namespace Getaway.Application.CQRS.User.Commands.DeleteUser
{
    public class DeleteUserHandler(IUserRepository userService) : IRequestHandler<DeleteUserCommand>
    {
        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //await Connections.UserServiceClient.DeleteUserAsync(new DeleteUserRequest() { Id = request.UserId });
                userService.DeleteUser(request.UserId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
