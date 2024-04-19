using MediatR;
using ApiGetaway.Core.Exceptions;
using ApiGetaway.Core.ConnectionSettings;
using ApiGetaway.Logic.UserLogic.Queries.GetUserById;

namespace ApiGetaway.Logic.UserLogic.Queries.GetUserByTag
{
    public class GetUserByTagHandler : IRequestHandler<GetUserByTagQuery, GetUserByTagReply>
    {
        public async Task<GetUserByTagReply> Handle(GetUserByTagQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await Connections.UserServiceClient.GetUserByTagAsync(new GetUserByTagRequest() { Tag = request.UserTag });
                return new GetUserByTagReply()
                {
                    FirstName = user.FirstName,
                    SecondName = user.SecondName,
                    LastName = user.LastName,
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
