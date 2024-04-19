using Getaway.Application.ServicesInterfaces;
using MediatR;

namespace Getaway.Application.CQRS.User.Commands.UpdateUser
{
    public class UpdateUserHandler(IUserRepository userService) : IRequestHandler<UpdateUserCommand>
    {
        public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {

                userService.UpdateUser(new Core.Entities.UserEntity()
                {
                    ID = request.UserId,
                    FirstName = request.FirstName,
                    SecondName = request.SecondName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber
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
