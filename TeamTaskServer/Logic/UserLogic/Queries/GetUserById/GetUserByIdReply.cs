namespace ApiGetaway.Logic.UserLogic.Queries.GetUserById
{
    public class GetUserByIdReply
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string Tag { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
