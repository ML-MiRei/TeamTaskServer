using Getaway.Application.ServicesInterfaces;
using Getaway.Core.Entities;
using Getaway.Core.Exceptions;
using MediatR;


namespace Getaway.Application.CQRS.User.Queries.GetUserById
{
    public class GetUserByIdHandler(IUserRepository userService) : IRequestHandler<GetUserByIdQuery, UserEntity>
    {
        public async Task<UserEntity> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await userService.GetUserById(request.UserId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new NotFoundException();
            }
        }
    }
}
