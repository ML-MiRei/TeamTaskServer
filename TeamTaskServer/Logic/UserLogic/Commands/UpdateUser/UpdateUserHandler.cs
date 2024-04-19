using ApiGetaway.Core.ConnectionSettings;
using MediatR;

namespace ApiGetaway.Logic.UserLogic.Commands.UpdateUser
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand>
    {
        public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await Connections.UserServiceClient.UpdateUserAsync(new UpdateUserRequest()
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    SecondName = request.SecondName,
                    Phone = request.PhoneNumber,
                    Id = request.UserId
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
