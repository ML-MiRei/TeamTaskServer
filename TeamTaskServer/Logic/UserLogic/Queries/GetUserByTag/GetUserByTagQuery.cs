using MediatR;

namespace ApiGetaway.Logic.UserLogic.Queries.GetUserById
{
    public class GetUserByTagQuery : IRequest<GetUserByTagReply>
    {
        public string UserTag { get; set; }
    }
}
