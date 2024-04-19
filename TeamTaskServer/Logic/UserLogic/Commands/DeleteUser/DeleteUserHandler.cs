using ApiGetaway.Core.ConnectionSettings;
using ApiGetaway.Core.Exceptions;
using ApiGetaway.Logic.UserLogic.Queries.GetUserById;
using MediatR;

namespace ApiGetaway.Logic.UserLogic.Commands.DeleteUser
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
    {
        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await Connections.UserServiceClient.DeleteUserAsync(new DeleteUserRequest() { Id = request.UserId});
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
