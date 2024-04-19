using MediatR;

namespace ApiGetaway.Logic.UserLogic.Commands.UpdateUser
{
    public class UpdateUserCommand: IRequest
    {
        public int UserId { get; set;}
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
