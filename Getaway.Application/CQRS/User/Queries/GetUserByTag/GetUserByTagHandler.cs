using MediatR;
using Getaway.Core.Entities;
using Getaway.Core.Exceptions;
using Getaway.Application.ServicesInterfaces;

namespace Getaway.Application.CQRS.User.Queries.GetUserByTag
{
    public class GetUserByTagHandler(IUserRepository userService) : IRequestHandler<GetUserByTagQuery, UserEntity>
    {
        public async Task<UserEntity> Handle(GetUserByTagQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await userService.GetUserByTag(request.UserTag);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new NotFoundException();
            }
        }
    }
}
