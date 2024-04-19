using MediatR;
using ApiGetaway.Core.Exceptions;
using ApiGetaway.Core.ConnectionSettings;

namespace ApiGetaway.Logic.UserLogic.Queries.GetUserById
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdReply>
    {
        public async Task<GetUserByIdReply> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await Connections.UserServiceClient.GetUserByIdAsync(new GetUserByIdRequest() { UserId = request.UserId });
                return new GetUserByIdReply()
                {
                    FirstName = user.FirstName,
                    SecondName = user.SecondName,
                    LastName = user.LastName,
                    Tag = user.Tag,
                    Email = user.Email,
                    PhoneNumber = user.Phone
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new NotFoundException();
            }
        }
    }
}
