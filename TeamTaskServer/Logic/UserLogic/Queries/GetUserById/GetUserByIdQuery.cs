using MediatR;

namespace ApiGetaway.Logic.UserLogic.Queries.GetUserById
{
    public class GetUserByIdQuery : IRequest<GetUserByIdReply>
    {
        public int UserId { get; set; }

    }
}
